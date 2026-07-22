using System.Net;
using System.Net.Http.Json;

namespace Part12_ElectiveBranches.Tests;

[Collection(Part12Collection.Name)]
[Trait("Category", "Docker")]
public sealed class MailpitDockerTests(Part12Fixture fixture)
{
    private void SkipIfUnavailable() =>
        Assert.SkipWhen(!fixture.IsAvailable, fixture.SkipReason ?? "Mailpit/PostgreSQL unavailable.");

    [Fact]
    public async Task ScheduleEmailIsDeliveredToMailpit()
    {
        SkipIfUnavailable();
        await using Part12Factory factory = fixture.CreateFactory();
        using HttpClient client = await CreateReadyClientAsync(factory);

        string idemKey = $"test-{Guid.NewGuid():N}";
        using HttpResponseMessage schedule = await client.PostAsJsonAsync(
            "/api/notifications/email",
            new { Recipient = "student@example.test", Subject = "Course notice W9", HtmlBody = "<p>Your enrollment is confirmed.</p>", TextBody = (string?)"Your enrollment is confirmed.", IdempotencyKey = idemKey });
        schedule.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.Accepted, schedule.StatusCode);
        ScheduleResponse? scheduled = await schedule.Content.ReadFromJsonAsync<ScheduleResponse>();
        Assert.NotNull(scheduled);

        // Wait for the scheduler to pick up the job and Mailpit to receive it.
        bool delivered = false;
        for (int i = 0; i < 60; i++)
        {
            await Task.Delay(500);
            EmailJobStatus? jobStatus = await client.GetFromJsonAsync<EmailJobStatus>($"/api/notifications/jobs/{scheduled!.JobId}");
            if (jobStatus!.State == "completed")
            {
                delivered = true;
                break;
            }
        }
        Assert.True(delivered, "Email job did not complete in time.");

        // Assert the email arrived in Mailpit via the HTTP API.
        using HttpClient http = new HttpClient { BaseAddress = new Uri($"http://localhost:{fixture.MailpitApiPort}") };
        MailpitMessages? messages = await http.GetFromJsonAsync<MailpitMessages>("/api/v1/messages?limit=10");
        Assert.NotNull(messages?.Messages);
        Assert.Contains(messages!.Messages, m => m.Subject == "Course notice W9");
    }

    [Fact]
    public async Task IdempotencyKeyPreventsDuplicateScheduling()
    {
        SkipIfUnavailable();
        await using Part12Factory factory = fixture.CreateFactory();
        using HttpClient client = await CreateReadyClientAsync(factory);

        string idemKey = $"idem-{Guid.NewGuid():N}";
        JsonContent requestContent = JsonContent.Create(new { Recipient = "student@example.test", Subject = "dup test", HtmlBody = "<p>dup</p>", TextBody = (string?)"dup" });
        client.DefaultRequestHeaders.Add("Idempotency-Key", idemKey);

        using HttpResponseMessage first = await client.PostAsync("/api/notifications/email", requestContent);
        first.EnsureSuccessStatusCode();
        ScheduleResponse? firstResult = await first.Content.ReadFromJsonAsync<ScheduleResponse>();

        using HttpResponseMessage second = await client.PostAsync("/api/notifications/email", requestContent);
        second.EnsureSuccessStatusCode();
        ScheduleResponse? secondResult = await second.Content.ReadFromJsonAsync<ScheduleResponse>();

        Assert.Equal(firstResult!.JobId, secondResult!.JobId);
    }

    private static async Task<HttpClient> CreateReadyClientAsync(Part12Factory factory)
    {
        HttpClient client = factory.CreateClient();
        try
        {
            for (int attempt = 0; attempt < 60; attempt++)
            {
                using HttpResponseMessage ready = await client.GetAsync("/health/ready");
                if (ready.IsSuccessStatusCode)
                {
                    return client;
                }
                await Task.Delay(200);
            }
            throw new TimeoutException("Part12 lab did not become ready.");
        }
        catch
        {
            client.Dispose();
            throw;
        }
    }

    private sealed record ScheduleResponse(Guid JobId);
    private sealed record EmailJobStatus(Guid JobId, string State, int Attempts, string? ProviderMessageId, DateTimeOffset ScheduledAt, DateTimeOffset? CompletedAt);
    private sealed record MailpitMessages(MailpitMessage[] Messages);
    private sealed record MailpitMessage(string Subject, MailpitAddress[] To);
    private sealed record MailpitAddress(string Address, string Name);
}
