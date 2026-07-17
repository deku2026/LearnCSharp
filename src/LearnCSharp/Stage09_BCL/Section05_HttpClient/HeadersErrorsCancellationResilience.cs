// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : HeadersErrorsCancellationResilience
// Topic id : stage09/section05/headers_errors_cancellation_resilience
//
// 步骤 5：headers、EnsureSuccessStatusCode、取消/超时、DelegatingHandler 重试（短超时、离线安全）

using System.Diagnostics;
using System.Net;
using System.Net.Http.Headers;
using LearnCSharp.Topics;
using Microsoft.Extensions.DependencyInjection;

namespace LearnCSharp.Stage09.Section05;

internal static class HeadersErrorsCancellationResilience
{
    /// <summary>Educational retry handler: at most one retry on 5xx / network-ish failures.</summary>
    private sealed class RetryOnceHandler : DelegatingHandler
    {
        public int Attempts { get; private set; }

        protected override async Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            Attempts++;
            try
            {
                HttpResponseMessage response = await base.SendAsync(request, cancellationToken);
                if ((int)response.StatusCode is >= 500 and < 600 && Attempts < 2)
                {
                    response.Dispose();
                    Attempts++;
                    await Task.Delay(20, cancellationToken);
                    return await base.SendAsync(CloneRequest(request), cancellationToken);
                }

                return response;
            }
            catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException)
            {
                if (Attempts >= 2)
                    throw;
                Attempts++;
                await Task.Delay(20, cancellationToken);
                return await base.SendAsync(CloneRequest(request), cancellationToken);
            }
        }

        private static HttpRequestMessage CloneRequest(HttpRequestMessage original)
        {
            HttpRequestMessage clone = new HttpRequestMessage(original.Method, original.RequestUri);
            foreach (KeyValuePair<string, IEnumerable<string>> header in original.Headers)
                clone.Headers.TryAddWithoutValidation(header.Key, header.Value);
            return clone;
        }
    }

    /// <summary>Offline-safe stub: always returns 503 then (via retry path) still 503 — no network.</summary>
    private sealed class StubFailHandler : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
            => Task.FromResult(new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)
            {
                RequestMessage = request,
                ReasonPhrase = "stub-offline"
            });
    }

    [LearnTopic("stage09/section05/headers_errors_cancellation_resilience")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== HeadersErrorsCancellationResilience ===");
        DemoHeaders().GetAwaiter().GetResult();
        DemoErrorsAndEnsureSuccess().GetAwaiter().GetResult();
        DemoCancellation().GetAwaiter().GetResult();
        DemoDelegatingHandlerRetry().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoHeaders()
    {
        Console.WriteLine("-- DefaultRequestHeaders + per-request headers --");
        using HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        try
        {
            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
            request.Headers.TryAddWithoutValidation("X-Request-Id", Guid.NewGuid().ToString("N"));
            using HttpResponseMessage response = await client.SendAsync(request);
            Console.WriteLine($"  Accept sent; status={(int)response.StatusCode}; server={response.Headers.Server}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network soft-fail: {ex.GetType().Name}");
        }
    }

    private static async Task DemoErrorsAndEnsureSuccess()
    {
        Console.WriteLine("-- status codes: check vs EnsureSuccessStatusCode (offline stub) --");
        using StubFailHandler handler = new StubFailHandler();
        using HttpClient client = new HttpClient(handler) { Timeout = TimeSpan.FromSeconds(1) };
        using HttpResponseMessage response = await client.GetAsync("https://offline.test/status");
        Debug.Assert(response.StatusCode == HttpStatusCode.ServiceUnavailable);
        Console.WriteLine($"  expected non-success: {(int)response.StatusCode} {response.ReasonPhrase}");
        try
        {
            response.EnsureSuccessStatusCode();
            Debug.Assert(false, "should have thrown");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"  EnsureSuccessStatusCode threw: {ex.GetType().Name}");
        }
    }

    private static async Task DemoCancellation()
    {
        Console.WriteLine("-- CancellationToken + short timeout --");
        using HttpClient client = new HttpClient { Timeout = TimeSpan.FromSeconds(2) };
        using CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(1));
        try
        {
            _ = await client.GetAsync("https://example.com/", cts.Token);
            Console.WriteLine("  completed before cancel (fast network)");
        }
        catch (Exception ex) when (ex is TaskCanceledException or OperationCanceledException or HttpRequestException)
        {
            Console.WriteLine($"  canceled/timeout as expected: {ex.GetType().Name}");
        }
    }

    private static async Task DemoDelegatingHandlerRetry()
    {
        Console.WriteLine("-- DelegatingHandler retry (offline-safe, no hang) --");
        RetryOnceHandler retry = new RetryOnceHandler { InnerHandler = new StubFailHandler() };
        ServiceCollection services = new ServiceCollection();
        services.AddHttpClient("resilient", c => c.Timeout = TimeSpan.FromSeconds(1))
            .ConfigurePrimaryHttpMessageHandler(() => new StubFailHandler())
            .AddHttpMessageHandler(() => new RetryOnceHandler());

        // Also exercise the standalone pipeline for attempt counting
        using HttpClient pipeline = new HttpClient(retry) { Timeout = TimeSpan.FromSeconds(1) };
        using HttpResponseMessage response = await pipeline.GetAsync("https://offline.test/retry");
        Debug.Assert(response.StatusCode == HttpStatusCode.ServiceUnavailable);
        Debug.Assert(retry.Attempts >= 2);
        Console.WriteLine($"  retry attempts={retry.Attempts}; final status={(int)response.StatusCode}");

        using ServiceProvider provider = services.BuildServiceProvider();
        IHttpClientFactory factory = provider.GetRequiredService<IHttpClientFactory>();
        using HttpClient client = factory.CreateClient("resilient");
        using HttpResponseMessage viaFactory = await client.GetAsync("https://offline.test/via-factory");
        Debug.Assert(viaFactory.StatusCode == HttpStatusCode.ServiceUnavailable);
        Console.WriteLine($"  AddHttpClient + handler pipeline status={(int)viaFactory.StatusCode}");
    }
}
