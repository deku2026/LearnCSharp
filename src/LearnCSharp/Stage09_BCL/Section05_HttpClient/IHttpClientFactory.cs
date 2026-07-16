// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : IHttpClientFactory
// Topic id : stage09/section05/ihttp_client_factory
//
// 步骤 3：IHttpClientFactory 概念 — 无 Microsoft.Extensions.Http 时用教育型迷你工厂

using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section05;

internal static class IHttpClientFactoryDemo
{
    /// <summary>
    /// Educational stand-in for Microsoft.Extensions.Http.IHttpClientFactory:
    /// named clients, shared handlers, short-lived HttpClient wrappers.
    /// </summary>
    private interface IMiniHttpClientFactory
    {
        HttpClient CreateClient(string name = "");
    }

    private sealed class MiniHttpClientFactory : IMiniHttpClientFactory, IDisposable
    {
        private readonly ConcurrentDictionary<string, Lazy<HttpMessageHandler>> _handlers = new(StringComparer.Ordinal);
        private readonly ConcurrentDictionary<string, Action<HttpClient>> _configure = new(StringComparer.Ordinal);

        public void Configure(string name, Action<HttpClient> configure)
            => _configure[name] = configure;

        public HttpClient CreateClient(string name = "")
        {
            HttpMessageHandler handler = _handlers.GetOrAdd(name, static _ =>
                new Lazy<HttpMessageHandler>(() => new SocketsHttpHandler
                {
                    PooledConnectionLifetime = TimeSpan.FromMinutes(2)
                })).Value;

            var client = new HttpClient(handler, disposeHandler: false)
            {
                Timeout = TimeSpan.FromSeconds(3)
            };
            if (_configure.TryGetValue(name, out Action<HttpClient>? cfg))
                cfg(client);
            return client; // short-lived wrapper; handler is pooled
        }

        public void Dispose()
        {
            foreach (Lazy<HttpMessageHandler> lazy in _handlers.Values)
            {
                if (lazy.IsValueCreated)
                    lazy.Value.Dispose();
            }
            _handlers.Clear();
        }
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
        Console.WriteLine("-- production: services.AddHttpClient / named / typed --");
        Console.WriteLine("  AddHttpClient() → IHttpClientFactory.CreateClient()");
        Console.WriteLine("  AddHttpClient(\"github\", c => c.BaseAddress = ...) → named");
        Console.WriteLine("  AddHttpClient<GitHubClient>() → typed client class");
        Console.WriteLine("  (this demo uses a BCL-only mini factory — no extra packages)");
        Debug.Assert(true);
    }

    private static async Task DemoFactoryCreate()
    {
        Console.WriteLine("-- mini factory: short-lived clients, shared handler --");
        using var factory = new MiniHttpClientFactory();
        factory.Configure("example", c =>
        {
            c.BaseAddress = new Uri("https://example.com/");
            c.DefaultRequestHeaders.UserAgent.ParseAdd("LearnCSharp-Stage09");
        });

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
