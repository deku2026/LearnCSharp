using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Kestrel limits (HTTP base) — configured via code; launchSettings sets URLs
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1024 * 1024; // 1 MB lab limit
    options.Limits.KeepAliveTimeout = TimeSpan.FromMinutes(2);
    options.Limits.RequestHeadersTimeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient("campus-catalog", (sp, client) =>
{
    var baseUrl = sp.GetRequiredService<IConfiguration>()["Downstream:CatalogBaseUrl"]
                  ?? "https://httpbin.org";
    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(10);
}).AddStandardResilienceHandler();

builder.Services.AddHttpClient<IStudentLookupClient, StudentLookupClient>(client =>
{
    client.BaseAddress = new Uri("https://httpbin.org/");
    client.Timeout = TimeSpan.FromSeconds(10);
});

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    lab = "Step10 Kestrel + IHttpClientFactory",
    note = "Named/typed clients avoid socket exhaustion; resilience handler attached"
}));

app.MapGet("/api/downstream/get", async (IHttpClientFactory factory, CancellationToken ct) =>
{
    var client = factory.CreateClient("campus-catalog");
    using var res = await client.GetAsync("get", ct);
    var body = await res.Content.ReadAsStringAsync(ct);
    return Results.Content(body, "application/json");
});

app.MapGet("/api/students/lookup/{id}", async (string id, IStudentLookupClient client, CancellationToken ct) =>
{
    var json = await client.EchoAsync(id, ct);
    return Results.Content(json, "application/json");
});

app.MapGet("/api/kestrel-info", (IConfiguration config) => Results.Ok(new
{
    maxRequestBodySize = 1024 * 1024,
    urlsFromConfig = config["ASPNETCORE_URLS"] ?? "(from launchSettings / defaults)"
}));

app.Run();

public partial class Program;

public interface IStudentLookupClient
{
    Task<string> EchoAsync(string id, CancellationToken ct);
}

public sealed class StudentLookupClient(HttpClient http) : IStudentLookupClient
{
    public async Task<string> EchoAsync(string id, CancellationToken ct)
    {
        // httpbin returns request metadata — stands in for a student service
        using var res = await http.GetAsync($"get?studentId={Uri.EscapeDataString(id)}", ct);
        res.EnsureSuccessStatusCode();
        return await res.Content.ReadAsStringAsync(ct);
    }
}
