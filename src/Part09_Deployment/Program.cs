var builder = WebApplication.CreateBuilder(args);
builder.Services.AddHealthChecks();
var app = builder.Build();
app.MapGet("/", () => Results.Ok(new { lab="Part09_Deployment", status="ok" }));
app.MapHealthChecks("/health");
app.Run();
public partial class Program;
