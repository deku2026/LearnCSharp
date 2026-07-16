// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : HeadersErrorsCancellationResilience
// Topic id : stage09/section05/headers_errors_cancellation_resilience
//
// 步骤 5：headers、EnsureSuccessStatusCode、取消/超时、简易重试（无 Polly 包）

using System.Diagnostics;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section05;

internal static class HeadersErrorsCancellationResilience
{
    private static readonly HttpClient s_client = new()
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    [LearnTopic("stage09/section05/headers_errors_cancellation_resilience")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== HeadersErrorsCancellationResilience ===");
        DemoHeaders().GetAwaiter().GetResult();
        DemoErrorsAndEnsureSuccess().GetAwaiter().GetResult();
        DemoCancellation().GetAwaiter().GetResult();
        DemoSimpleRetry().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoHeaders()
    {
        Console.WriteLine("-- DefaultRequestHeaders + per-request headers --");
        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            request.Headers.TryAddWithoutValidation("X-Request-Id", Guid.NewGuid().ToString("N"));
            using HttpResponseMessage response = await s_client.SendAsync(request);
            Console.WriteLine($"  Accept sent; status={(int)response.StatusCode}; server={response.Headers.Server}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network soft-fail: {ex.GetType().Name}");
        }
    }

    private static async Task DemoErrorsAndEnsureSuccess()
    {
        Console.WriteLine("-- status codes: check vs EnsureSuccessStatusCode --");
        try
        {
            using HttpResponseMessage response = await s_client.GetAsync("https://httpbin.org/status/404");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"  expected non-success: {(int)response.StatusCode} {response.ReasonPhrase}");
                Debug.Assert(response.StatusCode == HttpStatusCode.NotFound
                             || response.StatusCode is not 0);
            }
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"  EnsureSuccessStatusCode threw: {ex.Message}");
            }
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network soft-fail: {ex.GetType().Name}");
        }
    }

    private static async Task DemoCancellation()
    {
        Console.WriteLine("-- CancellationToken + short timeout --");
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
        try
        {
            _ = await s_client.GetAsync("https://example.com/", cts.Token);
            Console.WriteLine("  completed before cancel (fast network)");
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException or HttpRequestException)
        {
            Console.WriteLine($"  canceled/timeout as expected: {ex.GetType().Name}");
        }
    }

    private static async Task DemoSimpleRetry()
    {
        Console.WriteLine("-- naive retry loop (Polly would do this better) --");
        const int maxAttempts = 2;
        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                using HttpResponseMessage response = await s_client.GetAsync("https://example.com/");
                Console.WriteLine($"  attempt {attempt}: status={(int)response.StatusCode}");
                break;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
            {
                Console.WriteLine($"  attempt {attempt} failed: {ex.GetType().Name}");
                if (attempt == maxAttempts)
                    Console.WriteLine("  giving up soft (return 0)");
                else
                    await Task.Delay(50);
            }
        }
    }
}
