using Campus.ServiceDefaults;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using Part12_ElectiveBranches;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.AddCampusServiceDefaults();
builder.Services.AddProblemDetails();
builder.Services.AddSingleton<InboxStore>();
builder.Services.AddSingleton<KafkaProducerService>();
builder.Services.AddHostedService<KafkaConsumerService>();
builder.Services.AddSingleton<EmailJobStore>();
builder.Services.AddSingleton<SmtpEmailClient>();
builder.Services.AddHostedService<EmailSchedulerService>();
builder.Services.AddHealthChecks()
    .AddAsyncCheck("postgres", async ct =>
    {
        string? cs = builder.Configuration.GetConnectionString("Notifications");
        if (string.IsNullOrWhiteSpace(cs))
        {
            return HealthCheckResult.Degraded("Notifications DB not configured");
        }
        try
        {
            await using NpgsqlConnection connection = new NpgsqlConnection(cs);
            await connection.OpenAsync(ct);
            await using NpgsqlCommand command = connection.CreateCommand();
            command.CommandText = "SELECT 1";
            _ = await command.ExecuteScalarAsync(ct);
            return HealthCheckResult.Healthy();
        }
        catch (Exception ex) when (ex is NpgsqlException or TimeoutException)
        {
            return HealthCheckResult.Unhealthy("Notifications DB unavailable", ex);
        }
    }, ["ready"])
    .AddAsyncCheck("kafka", async ct =>
    {
        bool runConsumer = builder.Configuration.GetValue("Kafka:RunConsumer", true);
        if (!runConsumer)
        {
            return HealthCheckResult.Healthy("Kafka consumer disabled");
        }
        return await Task.FromResult(HealthCheckResult.Healthy());
    }, ["ready"]);

WebApplication app = builder.Build();
app.UseExceptionHandler();

await app.Services.GetRequiredService<InboxStore>().InitializeAsync(CancellationToken.None);

app.MapGet("/", () => Results.Ok(new
{
    lab = "Part12_ElectiveBranches",
    branches = new[]
    {
        "Kafka event stream + replay (campus.enrollment.activity.v1 + DLQ)",
        "Durable background email (Postgres job store + Mailpit SMTP)",
    },
    semantics = "Kafka: at-least-once with manual commit + idempotent inbox; Email: FOR UPDATE SKIP LOCKED + exponential backoff",
}));

RouteGroupBuilder kafka = app.MapGroup("/api/kafka");
kafka.MapPost("/enrollment-activity", async (
    EnrollmentActivityRequest request,
    KafkaProducerService producer,
    CancellationToken ct) =>
{
    EnrollmentActivityEvent evt = new EnrollmentActivityEvent(
        request.EnrollmentId == Guid.Empty ? Guid.NewGuid() : request.EnrollmentId,
        request.StudentId,
        request.SectionId,
        request.Status ?? "Confirmed",
        DateTimeOffset.UtcNow);
    KafkaProduceResult result = await producer.PublishAsync(evt, ct);
    return Results.Accepted($"/api/kafka/status", result);
});

kafka.MapGet("/status", (KafkaProducerService _) => Results.Ok(new KafkaStatusDto(
    KafkaTopics.EnrollmentActivity,
    Partitions: -1,
    ConsumerLag: -1,
    ConsumerGroup: builder.Configuration["Kafka:GroupId"] ?? "campus-w9-consumer")));

RouteGroupBuilder notifications = app.MapGroup("/api/notifications");
notifications.MapPost("/email", async (
    HttpContext ctx,
    ScheduleEmailRequest request,
    EmailJobStore store,
    CancellationToken ct) =>
{
    if (!request.Recipient.EndsWith("@example.test", StringComparison.OrdinalIgnoreCase))
    {
        return Results.BadRequest(new { error = "recipient.must_be_example_test" });
    }
    string? idemKey = ctx.Request.Headers["Idempotency-Key"].ToString();
    if (string.IsNullOrWhiteSpace(idemKey))
    {
        idemKey = null;
    }
    Guid jobId = await store.ScheduleAsync(
        request.Recipient, request.Subject, request.HtmlBody, request.TextBody, idemKey, ct);
    return Results.Accepted($"/api/notifications/jobs/{jobId}", new { jobId });
});

notifications.MapGet("/jobs/{id:guid}", async (Guid id, EmailJobStore store, CancellationToken ct) =>
{
    EmailJobStatus? status = await store.GetAsync(id, ct);
    return status is null ? Results.NotFound() : Results.Ok(status);
});

notifications.MapDelete("/jobs/{id:guid}", async (Guid id, EmailJobStore store, CancellationToken ct) =>
{
    bool cancelled = await store.CancelAsync(id, ct);
    return cancelled ? Results.Ok(new { cancelled = true }) : Results.NotFound();
});

app.MapHealthChecks("/health/live", new HealthCheckOptions
{
    Predicate = check => check.Tags.Contains("live"),
});
app.MapHealthChecks("/health/ready", new HealthCheckOptions
{
    Predicate = check => !check.Tags.Contains("live"),
});

app.Run();

public partial class Program;

public sealed record EnrollmentActivityRequest(
    Guid EnrollmentId,
    Guid StudentId,
    Guid SectionId,
    string? Status);
