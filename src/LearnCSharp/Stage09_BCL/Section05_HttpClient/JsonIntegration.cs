// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第5部分-HttpClient.md
// Stage    : Stage09_BCL
// Section  : Section05_HttpClient
// Item     : JsonIntegration
// Topic id : stage09/section05/json_integration
//
// 步骤 4：HTTP + System.Text.Json（GetFromJsonAsync / PostAsJsonAsync / 手动）

using System.Diagnostics;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section05;

internal static class JsonIntegration
{
    private sealed class Todo
    {
        public int UserId { get; set; }
        public int Id { get; set; }
        public string? Title { get; set; }
        public bool Completed { get; set; }
    }

    private static readonly HttpClient s_client = new()
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    private static readonly JsonSerializerOptions s_json = new()
    {
        PropertyNameCaseInsensitive = true
    };

    [LearnTopic("stage09/section05/json_integration")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== JsonIntegration ===");
        DemoLocalRoundTrip();
        DemoGetFromJson().GetAwaiter().GetResult();
        return 0;
    }

    private static void DemoLocalRoundTrip()
    {
        Console.WriteLine("-- local STJ prepare body for PostAsJsonAsync --");
        Todo todo = new Todo { UserId = 1, Title = "learn", Completed = false };
        string json = JsonSerializer.Serialize(todo, s_json);
        Todo? back = JsonSerializer.Deserialize<Todo>(json, s_json);
        Debug.Assert(back is { Title: "learn", UserId: 1 });
        using StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
        Debug.Assert(content.Headers.ContentType?.MediaType == "application/json");
        Console.WriteLine($"  prepared JSON length={json.Length}");
    }

    private static async Task DemoGetFromJson()
    {
        Console.WriteLine("-- GetFromJsonAsync (jsonplaceholder) --");
        try
        {
            Todo? todo = await s_client.GetFromJsonAsync<Todo>(
                "https://jsonplaceholder.typicode.com/todos/1", s_json);
            if (todo is not null)
            {
                Debug.Assert(todo.Id == 1);
                Console.WriteLine($"  todo id={todo.Id} title={todo.Title}");
            }

            // PostAsJsonAsync shape (may 404/soft-fail on some hosts)
            using HttpResponseMessage post = await s_client.PostAsJsonAsync(
                "https://jsonplaceholder.typicode.com/posts",
                new Todo { Title = "demo", UserId = 1 },
                s_json);
            Console.WriteLine($"  PostAsJsonAsync status={(int)post.StatusCode}");
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException
                                       or OperationCanceledException or JsonException or NotSupportedException)
        {
            Console.WriteLine($"  network/json soft-fail: {ex.GetType().Name}");
        }
    }
}
