using System.Net;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Step10_KestrelHttpClientFactory.Tests;

public sealed class HttpClientTests
{
    [Fact]
    public async Task Downstream_get_uses_named_client()
    {
        // Use custom handler to avoid real network flakiness
        await using var factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.ConfigureTestServices(services =>
            {
                services.AddHttpClient("campus-catalog")
                    .ConfigurePrimaryHttpMessageHandler(() => new StubHandler());
            });
        });

        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/downstream/get");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
        Assert.Contains("stub-ok", await res.Content.ReadAsStringAsync());
    }

    [Fact]
    public async Task Kestrel_info_endpoint()
    {
        await using var factory = new WebApplicationFactory<Program>();
        var client = factory.CreateClient();
        var res = await client.GetAsync("/api/kestrel-info");
        Assert.Equal(HttpStatusCode.OK, res.StatusCode);
    }

    private sealed class StubHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("{\"message\":\"stub-ok\"}")
            });
    }
}
