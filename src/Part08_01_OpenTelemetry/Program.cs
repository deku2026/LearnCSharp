using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHttpClient();
builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("Part08_01_OpenTelemetry"))
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddOtlpExporter());
builder.Logging.AddOpenTelemetry(o => { o.IncludeFormattedMessage = true; o.IncludeScopes = true; });

var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab="Part08_01 OpenTelemetry", otlp="http://localhost:4317", dashboard="http://localhost:18888" }));
app.MapGet("/api/work", async (IHttpClientFactory http) =>
{
    var client = http.CreateClient();
    try { await client.GetAsync("https://httpbin.org/get"); } catch { }
    return Results.Ok(new { done = true });
});
app.Run();
public partial class Program;
