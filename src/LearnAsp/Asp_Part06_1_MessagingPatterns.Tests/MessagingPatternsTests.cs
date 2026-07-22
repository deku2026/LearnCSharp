using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;

namespace Part06_1_MessagingPatterns.Tests;

[Collection(MessagingPatternsCollection.Name)]
[Trait("Category", "Docker")]
public sealed class MessagingPatternsTests(MessagingPatternsFixture fixture)
{
    [Fact]
    public async Task EnrollmentAndOutboxAreCommittedAtomically()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        await using W7PatternsFactory factory = fixture.CreateFactory();
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/messaging/enrollments",
            new CreateEnrollmentCommand(Guid.NewGuid(), Guid.NewGuid()));

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        EnrollmentRecord? enrollment = await response.Content.ReadFromJsonAsync<EnrollmentRecord>();
        Assert.NotNull(enrollment);
        await using MessagingDbContext database = CreateDatabase();
        Assert.True(await database.Enrollments.AnyAsync(row => row.Id == enrollment.Id));
        OutboxMessage message = await database.OutboxMessages.SingleAsync();
        Assert.Equal("campus.enrollment.requested.v1", message.Type);
        Assert.Null(message.ProcessedOnUtc);
    }

    [Fact]
    public async Task RelayCrashRedeliversAndInboxAbsorbsDuplicate()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        await using W7PatternsFactory factory = fixture.CreateFactory();
        using HttpClient client = factory.CreateClient();
        OutboxMessage queued = await QueueMessageAsync(client, "campus.enrollment.confirmed", "{}");

        using HttpResponseMessage crashed = await client.PostAsync(
            "/api/messaging/outbox/dispatch?batchSize=1&crashAfterPublish=true",
            null);
        Assert.Equal(HttpStatusCode.InternalServerError, crashed.StatusCode);
        using HttpResponseMessage dispatched = await client.PostAsync(
            "/api/messaging/outbox/dispatch?batchSize=1",
            null);
        dispatched.EnsureSuccessStatusCode();

        List<PublishedMessage>? published = await client.GetFromJsonAsync<List<PublishedMessage>>(
            "/api/messaging/published");
        Assert.Equal(2, published!.Count(message => message.MessageId == queued.Id));

        ReceiveInboxMessageRequest inboxRequest = new ReceiveInboxMessageRequest(
            queued.Id,
            Guid.NewGuid(),
            queued.Type,
            queued.ContentJson);
        using HttpResponseMessage first = await client.PostAsJsonAsync(
            "/api/messaging/inbox",
            inboxRequest);
        using HttpResponseMessage second = await client.PostAsJsonAsync(
            "/api/messaging/inbox",
            inboxRequest);
        InboxResult? firstResult = await first.Content.ReadFromJsonAsync<InboxResult>();
        InboxResult? secondResult = await second.Content.ReadFromJsonAsync<InboxResult>();
        Assert.True(firstResult!.Accepted);
        Assert.True(secondResult!.Duplicate);
        List<NotificationReceipt>? receipts = await client.GetFromJsonAsync<List<NotificationReceipt>>(
            "/api/messaging/notification-receipts");
        Assert.Single(receipts!);
    }

    [Fact]
    public async Task ConcurrentDispatchersUseSkipLockedWithoutDuplicateClaims()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        await using W7PatternsFactory factory = fixture.CreateFactory();
        using HttpClient firstClient = factory.CreateClient();
        using HttpClient secondClient = factory.CreateClient();
        for (int index = 0; index < 20; index++)
        {
            await QueueMessageAsync(firstClient, $"campus.lab.{index}", "{}");
        }

        HttpResponseMessage[] dispatches = await Task.WhenAll(
            firstClient.PostAsync("/api/messaging/outbox/dispatch?batchSize=10", null),
            secondClient.PostAsync("/api/messaging/outbox/dispatch?batchSize=10", null));
        foreach (HttpResponseMessage? response in dispatches)
        {
            using (response)
            {
                response.EnsureSuccessStatusCode();
            }
        }

        List<PublishedMessage>? published = await firstClient.GetFromJsonAsync<List<PublishedMessage>>(
            "/api/messaging/published");
        Assert.Equal(20, published!.Count);
        Assert.Equal(20, published.Select(message => message.MessageId).Distinct().Count());
        await using MessagingDbContext database = CreateDatabase();
        Assert.Equal(20, await database.OutboxMessages.CountAsync(
            message => message.ProcessedOnUtc != null));
    }

    [Fact]
    public async Task TransientFailuresBackOffAndPoisonMessagesGoToDeadLetter()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        await using W7PatternsFactory factory = fixture.CreateFactory();
        using HttpClient client = factory.CreateClient();
        OutboxMessage poison = await QueueMessageAsync(
            client,
            "campus.lab.poison",
            "{}",
            "poison");
        OutboxMessage transient = await QueueMessageAsync(
            client,
            "campus.lab.transient",
            "{}",
            "transient",
            2);

        await DispatchAsync(client);
        await Task.Delay(50);
        await DispatchAsync(client);
        await Task.Delay(100);
        await DispatchAsync(client);

        List<DeadLetterMessage>? deadLetters = await client.GetFromJsonAsync<List<DeadLetterMessage>>(
            "/api/messaging/dead-letters");
        DeadLetterMessage deadLetter = Assert.Single(deadLetters!);
        Assert.Equal(poison.Id, deadLetter.OriginalMessageId);
        List<PublishedMessage>? published = await client.GetFromJsonAsync<List<PublishedMessage>>(
            "/api/messaging/published");
        Assert.Contains(published!, message => message.MessageId == transient.Id);
    }

    [Fact]
    public async Task SagaCompensatesPaymentWhenSeatReservationFails()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        await using W7PatternsFactory factory = fixture.CreateFactory();
        using HttpClient client = factory.CreateClient();
        using HttpResponseMessage started = await client.PostAsJsonAsync(
            "/api/messaging/sagas",
            new StartSagaRequest(Guid.NewGuid()));
        started.EnsureSuccessStatusCode();
        EnrollmentSaga? saga = await started.Content.ReadFromJsonAsync<EnrollmentSaga>();

        using HttpResponseMessage payment = await client.PostAsJsonAsync(
            $"/api/messaging/sagas/{saga!.Id}/payment",
            new SagaStepResult(true));
        payment.EnsureSuccessStatusCode();
        using HttpResponseMessage seat = await client.PostAsJsonAsync(
            $"/api/messaging/sagas/{saga.Id}/seat",
            new SagaStepResult(false, "Section is full."));
        EnrollmentSaga? compensating = await seat.Content.ReadFromJsonAsync<EnrollmentSaga>();
        Assert.Equal(SagaStates.CompensatingPayment, compensating!.State);
        Assert.True(compensating.PaymentReserved);

        using HttpResponseMessage compensation = await client.PostAsync(
            $"/api/messaging/sagas/{saga.Id}/compensation-completed",
            null);
        EnrollmentSaga? compensated = await compensation.Content.ReadFromJsonAsync<EnrollmentSaga>();
        Assert.Equal(SagaStates.Compensated, compensated!.State);
        Assert.False(compensated.PaymentReserved);
        await using MessagingDbContext database = CreateDatabase();
        Assert.Contains(
            await database.OutboxMessages.Select(message => message.Type).ToListAsync(),
            type => type == "campus.payment.refund-requested.v1");
    }

    private async Task<OutboxMessage> QueueMessageAsync(
        HttpClient client,
        string type,
        string payload,
        string failureMode = "none",
        int failuresBeforeSuccess = 0)
    {
        using HttpResponseMessage response = await client.PostAsJsonAsync(
            "/api/messaging/outbox",
            new QueueLabMessageRequest(
                type,
                payload,
                failureMode,
                failuresBeforeSuccess));
        response.EnsureSuccessStatusCode();
        return (await response.Content.ReadFromJsonAsync<OutboxMessage>())!;
    }

    private static async Task DispatchAsync(HttpClient client)
    {
        using HttpResponseMessage response = await client.PostAsync(
            "/api/messaging/outbox/dispatch?batchSize=20",
            null);
        response.EnsureSuccessStatusCode();
    }

    private MessagingDbContext CreateDatabase()
    {
        DbContextOptions<MessagingDbContext> options = new DbContextOptionsBuilder<MessagingDbContext>()
            .UseNpgsql(fixture.ConnectionString)
            .Options;
        return new MessagingDbContext(options);
    }

    private void SkipIfUnavailable()
    {
        Assert.SkipWhen(
            !fixture.IsAvailable,
            fixture.SkipReason ?? "PostgreSQL is unavailable.");
    }
}
