using System.Net;
using System.Net.Http.Json;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace Part08_2_TroubleshootingProcess.Tests;

public sealed class TroubleshootingApiTests :
    IClassFixture<TroubleshootingApiTests.TroubleshootingFactory>
{
    private readonly TroubleshootingFactory _factory;

    public TroubleshootingApiTests(TroubleshootingFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task DownstreamEndpointAppliesBoundedDelay()
    {
        using HttpClient client = _factory.CreateClient();
        Guid workId = Guid.NewGuid();

        DownstreamResponse? response = await client.GetFromJsonAsync<DownstreamResponse>(
            $"/api/downstream/{workId}?delayMs=3000");

        Assert.Equal(workId, response!.WorkId);
        Assert.Equal(2000, response.DelayMs);
        Assert.Equal("completed", response.Status);
    }

    [Fact]
    public async Task FaultLabRejectsMissingToken()
    {
        using HttpClient client = _factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/lab/slow?delayMs=1");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task FaultLabIsNotDiscoverableWhenDisabled()
    {
        using WebApplicationFactory<Program> baseFactory = new WebApplicationFactory<Program>();
        using WebApplicationFactory<Program> factory = baseFactory.WithWebHostBuilder(builder =>
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Troubleshooting:FaultInjectionEnabled"] = "false",
                        ["OTEL_EXPORTER_OTLP_ENDPOINT"] = null,
                    })));
        using HttpClient client = factory.CreateClient();

        using HttpResponseMessage response = await client.GetAsync("/lab/slow?delayMs=1");

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task AuthorizedFaultLabBoundsInputsAndReleasesMemory()
    {
        using HttpClient client = _factory.CreateClient();
        client.DefaultRequestHeaders.Add("X-Lab-Token", "test-lab-token");

        using HttpResponseMessage allocate = await client.PostAsync("/lab/memory?megabytes=100", null);
        allocate.EnsureSuccessStatusCode();
        MemoryResponse? allocated = await allocate.Content.ReadFromJsonAsync<MemoryResponse>();
        Assert.Equal(64, allocated!.RetainedMegabytes);

        using HttpResponseMessage release = await client.DeleteAsync("/lab/memory");
        release.EnsureSuccessStatusCode();
        MemoryResponse? released = await release.Content.ReadFromJsonAsync<MemoryResponse>();
        Assert.Equal(0, released!.RetainedMegabytes);
    }

    public sealed class TroubleshootingFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureAppConfiguration((_, configuration) =>
                configuration.AddInMemoryCollection(
                    new Dictionary<string, string?>
                    {
                        ["Troubleshooting:FaultInjectionEnabled"] = "true",
                        ["Troubleshooting:LabToken"] = "test-lab-token",
                        ["ConnectionStrings:Troubleshooting"] = null,
                        ["OTEL_EXPORTER_OTLP_ENDPOINT"] = null,
                    }));
        }
    }

    private sealed record DownstreamResponse(
        Guid WorkId,
        int DelayMs,
        string Status);

    private sealed record MemoryResponse(int RetainedMegabytes);
}
