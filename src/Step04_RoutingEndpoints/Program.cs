using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

var students = new Dictionary<int, string>
{
    [1001] = "张三",
    [1002] = "李四"
};

app.MapGet("/", () => Results.Ok(new { lab = "Step04 Routing & Endpoints" }));

// Route constraints
app.MapGet("/students/{id:int:min(1)}", (int id) =>
    students.TryGetValue(id, out var name)
        ? Results.Ok(new { id, name })
        : Results.NotFound());

// Regex constraint example (student number)
app.MapGet("/students/by-number/{number:length(10)}", (string number) =>
{
    if (!number.All(char.IsDigit))
    {
        return Results.NotFound();
    }

    return Results.Ok(new { studentNumber = number });
});

// Catch-all / optional
app.MapGet("/files/{*path}", (string path) => Results.Ok(new { path }));

// Route groups + metadata
var api = app.MapGroup("/api/v1").WithTags("Campus");

api.MapGet("/health", () => Results.Ok(new { status = "healthy" }))
    .WithName("CampusHealth")
    .WithSummary("Campus health probe");

api.MapGet("/students", () => students.Select(kv => new { id = kv.Key, name = kv.Value }));

// LinkGenerator: generate URI by endpoint name
app.MapGet("/links/health", (LinkGenerator links, HttpContext http) =>
{
    var uri = links.GetUriByName(http, "CampusHealth", values: null);
    return Results.Ok(new { healthUri = uri });
});

// Endpoint metadata inspection
app.MapGet("/debug/endpoint", (HttpContext http) =>
{
    var endpoint = http.GetEndpoint();
    return Results.Ok(new
    {
        displayName = endpoint?.DisplayName,
        metadata = endpoint?.Metadata.Select(m => m.GetType().Name).ToArray()
    });
});

// Parameter transformers / defaults
app.MapGet("/catalog/{category=books}", (string category) => Results.Ok(new { category }));

app.Run();

public partial class Program;
