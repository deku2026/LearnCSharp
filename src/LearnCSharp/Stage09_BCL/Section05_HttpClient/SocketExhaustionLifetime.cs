// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : SocketExhaustionLifetime
// Topic id : stage09/section05/socket_exhaustion_lifetime
//
// 步骤 2：勿 per-request new；勿永久单例无刷新；PooledConnectionLifetime 正解

using System.Diagnostics;
using System.Net.Http;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section05;

internal static class SocketExhaustionLifetime
{
    // ✅ long-lived client + pooled connection lifetime (DNS refresh)
    private static readonly HttpClient s_shared = CreateSharedClient();

    private static HttpClient CreateSharedClient()
    {
        var handler = new SocketsHttpHandler
        {
            PooledConnectionLifetime = TimeSpan.FromMinutes(15),
            MaxConnectionsPerServer = 20
        };
        return new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(3) };
    }

    [LearnTopic("stage09/section05/socket_exhaustion_lifetime")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== SocketExhaustionLifetime ===");
        DemoAntiPatternsExplained();
        DemoCorrectSharedClient().GetAwaiter().GetResult();
        return 0;
    }

    private static void DemoAntiPatternsExplained()
    {
        Console.WriteLine("-- anti-patterns (do not copy) --");
        Console.WriteLine("  ❌ using var c = new HttpClient() per request → sockets TIME_WAIT → exhaustion");
        Console.WriteLine("  ❌ static HttpClient forever with default handler → stale DNS");
        Console.WriteLine("  ✅ static/singleton + SocketsHttpHandler.PooledConnectionLifetime");
        Console.WriteLine("  ✅ or IHttpClientFactory (handler rotation)");
        Debug.Assert(s_shared is not null);
    }

    private static async Task DemoCorrectSharedClient()
    {
        Console.WriteLine("-- shared client with PooledConnectionLifetime --");
        try
        {
            using HttpResponseMessage response = await s_shared.GetAsync("https://example.com/");
            Console.WriteLine($"  shared GET status={(int)response.StatusCode}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network soft-fail: {ex.GetType().Name}");
        }
    }
}
