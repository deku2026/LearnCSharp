// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : HttpClientBasics
// Topic id : stage09/section05/http_client_basics
//
// 步骤 1：HttpClient Get/Post/SendAsync（短超时；网络失败仍 return 0）

using System.Diagnostics;
using System.Net;
using System.Text;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section05;

internal static class HttpClientBasics
{
    private static readonly HttpClient s_client = new()
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    [LearnTopic("stage09/section05/http_client_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== HttpClientBasics ===");
        DemoGet().GetAwaiter().GetResult();
        DemoPostAndSend().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoGet()
    {
        Console.WriteLine("-- GetAsync / GetStringAsync (example.com) --");
        try
        {
            using HttpResponseMessage response = await s_client.GetAsync("https://example.com/");
            string body = await response.Content.ReadAsStringAsync();
            Debug.Assert(response.StatusCode is HttpStatusCode.OK or HttpStatusCode.MovedPermanently or HttpStatusCode.Found
                || (int)response.StatusCode is >= 200 and < 500);
            Console.WriteLine($"  status={(int)response.StatusCode}; body length={body.Length}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network unavailable/timeout: {ex.GetType().Name} — demo continues");
        }
    }

    private static async Task DemoPostAndSend()
    {
        Console.WriteLine("-- PostAsync + SendAsync with HttpRequestMessage --");
        try
        {
            using StringContent content = new StringContent("""{"ping":true}""", Encoding.UTF8, "application/json");
            // httpbin may be flaky; treat any network failure as soft skip
            using HttpResponseMessage post = await s_client.PostAsync("https://httpbin.org/post", content);
            Console.WriteLine($"  POST status={(int)post.StatusCode}");

            using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, "https://example.com/");
            request.Headers.TryAddWithoutValidation("X-Demo", "LearnCSharp");
            using HttpResponseMessage sent = await s_client.SendAsync(request);
            Console.WriteLine($"  SendAsync status={(int)sent.StatusCode}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException or OperationCanceledException)
        {
            Console.WriteLine($"  network soft-fail: {ex.GetType().Name}");
        }
    }
}
