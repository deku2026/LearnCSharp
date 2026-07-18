var builder = WebApplication.CreateBuilder(args);
builder.AddCampusServiceDefaults();

var app = builder.Build();
app.MapCampusDefaultEndpoints();

app.MapGet("/", () => Results.Ok(new
{
    lab = "Part10 Aspire — sample API with Service Defaults",
    otlp = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT") ?? "(unset)",
    dashboard = "http://localhost:18888"
}));

app.MapGet("/api/students/ping", () => Results.Ok(new
{
    studentNumber = "2024001001",
    fullName = "张三"
}));

app.Run();

public partial class Program;

namespace Part10_Aspire.Api
{
    public sealed class AssemblyMarker;
}
