using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Part06_2_MessagingTools;
using Part07_DistributedComm.Grpc;

namespace Part07_DistributedComm.Tests;

[Collection(DistributedCollection.Name)]
[Trait("Category", "Docker")]
public sealed class DistributedCommTests(DistributedFixture fixture)
{
    [Fact]
    public async Task GatewayOffloadsAuthenticationAndBackendsRejectDirectClients()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        using HttpClient anonymousGateway = fixture.CreateGatewayClient();
        using HttpResponseMessage unauthorized = await anonymousGateway.GetAsync(
            $"/api/catalog/courses/{CatalogStore.SeedCourseId}");
        Assert.Equal(HttpStatusCode.Unauthorized, unauthorized.StatusCode);

        using HttpClient directEnrollment = new HttpClient
        {
            BaseAddress = new Uri(fixture.EnrollmentUrl),
        };
        directEnrollment.DefaultRequestHeaders.Add(
            "X-Campus-User",
            "spoofed-user");
        using HttpResponseMessage direct = await directEnrollment.PostAsJsonAsync(
            "enrollments",
            new CreateDistributedEnrollment(Guid.NewGuid(), CatalogStore.SeedCourseId));
        Assert.Equal(HttpStatusCode.Forbidden, direct.StatusCode);
    }

    [Fact]
    public async Task CapstoneFlowUsesGatewayGrpcOutboxRabbitAndIdempotentNotices()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        using HttpClient gateway = CreateAuthenticatedGatewayClient("student-42");
        gateway.DefaultRequestHeaders.Add("X-Correlation-ID", "capstone-flow-001");

        CatalogCourse? course = await gateway.GetFromJsonAsync<CatalogCourse>(
            $"/api/catalog/courses/{CatalogStore.SeedCourseId}");
        Assert.Equal("CS-CAPSTONE", course!.Code);

        using HttpResponseMessage created = await gateway.PostAsJsonAsync(
            "/api/enrollments",
            new CreateDistributedEnrollment(Guid.NewGuid(), course.Id));
        string createdBody = await created.Content.ReadAsStringAsync();
        Assert.True(
            created.StatusCode == HttpStatusCode.Accepted,
            $"status={(int)created.StatusCode}; body={createdBody}{Environment.NewLine}{fixture.Diagnostics()}");
        DistributedEnrollment? enrollment = await created.Content.ReadFromJsonAsync<DistributedEnrollment>();
        Assert.Equal("student-42", enrollment!.RequestedBy);
        Assert.Equal("capstone-flow-001", enrollment.CorrelationId);

        RabbitNotification notification = await WaitForNotificationAsync(gateway);
        Assert.Equal(enrollment.Id, notification.EnrollmentId);
        Assert.Contains("CS-CAPSTONE", notification.Payload, StringComparison.Ordinal);
    }

    [Fact]
    public async Task GrpcContractSupportsUnaryAndServerStreaming()
    {
        SkipIfUnavailable();
        using GrpcChannel channel = GrpcChannel.ForAddress(
            fixture.CatalogGrpcUrl,
            new GrpcChannelOptions
            {
                HttpHandler = new SocketsHttpHandler { UseProxy = false },
            });
        Catalog.CatalogClient client = new Catalog.CatalogClient(channel);

        CourseReply course;
        try
        {
            course = await client.GetCourseAsync(new GetCourseRequest
            {
                CourseId = CatalogStore.SeedCourseId.ToString(),
            });
        }
        catch (RpcException ex)
        {
            Assert.Fail($"{ex}{Environment.NewLine}{fixture.Diagnostics()}");
            throw;
        }
        Assert.Equal("CS-CAPSTONE", course.Code);
        Assert.Equal(30, course.AvailableSeats);

        using AsyncServerStreamingCall<AvailabilityReply> stream = client.WatchAvailability(new WatchAvailabilityRequest
        {
            CourseId = CatalogStore.SeedCourseId.ToString(),
            Samples = 3,
        });
        List<AvailabilityReply> samples = new List<AvailabilityReply>();
        while (await stream.ResponseStream.MoveNext(CancellationToken.None))
        {
            samples.Add(stream.ResponseStream.Current);
        }

        Assert.Equal(3, samples.Count);
        Assert.All(samples, sample => Assert.Equal(30, sample.AvailableSeats));
    }

    [Fact]
    public async Task StandardResilienceRetriesThenOpensCircuit()
    {
        SkipIfUnavailable();
        await fixture.ResetAsync();
        using HttpClient notices = new HttpClient { BaseAddress = new Uri(fixture.NoticesUrl) };
        using HttpResponseMessage configure = await notices.PostAsync(
            "internal/fault/configure/20",
            null);
        configure.EnsureSuccessStatusCode();
        using HttpClient gateway = CreateAuthenticatedGatewayClient("resilience-tester");

        using HttpResponseMessage first = await gateway.GetAsync(
            "/api/enrollments/resilience/notices");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, first.StatusCode);
        NoticesProbeResult? afterFirst = await notices.GetFromJsonAsync<NoticesProbeResult>(
            "internal/fault/stats");
        Assert.Equal(2, afterFirst!.RequestCount);

        using HttpResponseMessage second = await gateway.GetAsync(
            "/api/enrollments/resilience/notices");
        Assert.Equal(HttpStatusCode.ServiceUnavailable, second.StatusCode);
        NoticesProbeResult? afterSecond = await notices.GetFromJsonAsync<NoticesProbeResult>(
            "internal/fault/stats");
        Assert.Equal(
            afterFirst.RequestCount,
            afterSecond!.RequestCount);
    }

    [Fact]
    public async Task EveryProcessExposesSeparateLiveAndReadyProbes()
    {
        SkipIfUnavailable();
        foreach (string? baseUrl in new[]
                 {
                     fixture.CatalogUrl,
                     fixture.EnrollmentUrl,
                     fixture.NoticesUrl,
                     fixture.GatewayUrl,
                 })
        {
            using HttpClient client = new HttpClient { BaseAddress = new Uri(baseUrl) };
            using HttpResponseMessage live = await client.GetAsync("health/live");
            using HttpResponseMessage ready = await client.GetAsync("health/ready");
            Assert.Equal(HttpStatusCode.OK, live.StatusCode);
            Assert.Equal(HttpStatusCode.OK, ready.StatusCode);
        }
    }

    private HttpClient CreateAuthenticatedGatewayClient(string subject)
    {
        HttpClient client = fixture.CreateGatewayClient();
        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", IssueToken(subject));
        return client;
    }

    private static string IssueToken(string subject)
    {
        SecurityTokenDescriptor descriptor = new SecurityTokenDescriptor
        {
            Issuer = DistributedFixture.Issuer,
            Audience = DistributedFixture.Audience,
            Subject = new ClaimsIdentity(
            [
                new Claim("sub", subject),
                new Claim(ClaimTypes.NameIdentifier, subject),
            ]),
            IssuedAt = DateTime.UtcNow.AddSeconds(-1),
            NotBefore = DateTime.UtcNow.AddSeconds(-1),
            Expires = DateTime.UtcNow.AddMinutes(5),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(DistributedFixture.SigningKey)),
                SecurityAlgorithms.HmacSha256),
        };
        return new JsonWebTokenHandler().CreateToken(descriptor);
    }

    private static async Task<RabbitNotification> WaitForNotificationAsync(
        HttpClient gateway)
    {
        for (int attempt = 0; attempt < 50; attempt++)
        {
            List<RabbitNotification>? notifications = await gateway.GetFromJsonAsync<List<RabbitNotification>>(
                "/api/notices/notifications");
            if (notifications?.Count > 0)
            {
                return notifications[0];
            }

            await Task.Delay(100);
        }

        throw new TimeoutException("Capstone event did not reach Notices.");
    }

    private void SkipIfUnavailable()
    {
        Assert.SkipWhen(
            !fixture.IsAvailable,
            fixture.SkipReason ?? fixture.Diagnostics());
    }
}
