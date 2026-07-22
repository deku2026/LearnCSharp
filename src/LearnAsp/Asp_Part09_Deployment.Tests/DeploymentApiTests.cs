using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Mvc.Testing;

namespace Part09_Deployment.Tests;

public sealed class DeploymentApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public DeploymentApiTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task AppReportsRevisionWithoutLeakingSecrets()
    {
        using HttpClient client = _factory.CreateClient();

        DeploymentResponse? result = await client.GetFromJsonAsync<DeploymentResponse>(
            "/api/deployment/configuration");

        Assert.Equal("local", result!.Revision);
        Assert.Contains("never returned", result.Secrets, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData("/health/live")]
    [InlineData("/health/ready")]
    public async Task ContainerProbesAreIndependent(string path)
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync(path);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    private sealed record DeploymentResponse(string Revision, string Secrets);
}
