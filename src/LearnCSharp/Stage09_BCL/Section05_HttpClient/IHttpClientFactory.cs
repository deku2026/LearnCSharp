// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : IHttpClientFactory
// Topic id : stage09/section05/ihttp_client_factory
//
// 步骤 3：真实 AddHttpClient / IHttpClientFactory（Microsoft.Extensions.Http）

using System.Diagnostics;
using LearnCSharp.Topics;
using Microsoft.Extensions.DependencyInjection;

namespace LearnCSharp.Stage09.Section05;

internal static class IHttpClientFactoryDemo
{
    private sealed class ExampleApiClient(HttpClient http)
    {
        public HttpClient Http { get; } = http;
    }

    [LearnTopic("stage09/section05/ihttp_client_factory")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== IHttpClientFactory ===");
        DemoNamedAndTypedPatterns();
        DemoFactoryCreate().GetAwaiter().GetResult();
        return 0;
    }

    private static void DemoNamedAndTypedPatterns()
    {
        Console.WriteLine("-- AddHttpClient / named / typed --");
        ServiceCollection services = new ServiceCollection();
        services.AddHttpClient("example", c =>
        {
            c.BaseAddress = new Uri("https://example.com/");
            c.Timeout = TimeSpan.FromSeconds(2);
            c.DefaultRequestHeaders.UserAgent.ParseAdd("LearnCSharp-Stage09");
        });
        services.AddHttpClient<ExampleApiClient>(c =>
        {
            c.BaseAddress = new Uri("https://example.com/");
            c.Timeout = TimeSpan.FromSeconds(2);
        });

        using ServiceProvider provider = services.BuildServiceProvider();
        IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
        using HttpClient named = factory.CreateClient("example");
        Debug.Assert(named.BaseAddress == new Uri("https://example.com/"));
        ExampleApiClient typed = provider.GetRequiredService<ExampleApiClient>();
        Debug.Assert(typed.Http.BaseAddress is not null);
        Console.WriteLine("  named CreateClient(\"example\") + typed ExampleApiClient OK");
    }

    private static async Task DemoFactoryCreate()
    {
        Console.WriteLine("-- factory CreateClient GET (network soft-skip) --");
        ServiceCollection services = new ServiceCollection();
        services.AddHttpClient("example", c =>
        {
            c.BaseAddress = new Uri("https://example.com/");
            c.Timeout = TimeSpan.FromSeconds(2);
            c.DefaultRequestHeaders.UserAgent.ParseAdd("LearnCSharp-Stage09");
        });
        using ServiceProvider provider = services.BuildServiceProvider();
        IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();

        try
        {
            using HttpClient client = factory.CreateClient("example");
            using HttpResponseMessage response = await client.GetAsync("/");
            Console.WriteLine($"  named 'example' status={(int)response.StatusCode}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network soft-fail: {ex.GetType().Name}");
        }
    }
}
