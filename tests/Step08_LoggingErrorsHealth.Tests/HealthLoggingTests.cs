using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Step08_LoggingErrorsHealth.Tests;

public sealed class HealthLoggingTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public HealthLoggingTests(WebApplicationFactory<Program> factory) => _client = factory.CreateClient();

    [Fact]
    public async Task Live_is_healthy()
    {
        var res = await _client.GetAsync("/health/live");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Ready_checks_postgres_and_redis_when_docker_up()
    {
        var res = await _client.GetAsync("/health/ready");
        // Docker stack is expected running per environment setup
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    [Fact]
    public async Task Boom_returns_problem_and_correlation_header_echo()
    {
        using var req = new HttpRequestMessage(HttpMethod.Get, "/api/boom");
        req.Headers.Add("X-Correlation-Id", "corr-test-1");
        var res = await _client.SendAsync(req);
        Assert.Equal(HttpStatusCode.InternalServerError, res.StatusCode);
        Assert.True(res.Headers.TryGetValues("X-Correlation-Id", out var values));
        Assert.Contains("corr-test-1", values!);
    }
}
