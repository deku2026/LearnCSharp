// Step 01 · Hosting & startup model (ASP.NetStudy 步骤1)
// Four phases: CreateBuilder → Build → configure pipeline → Run

using Step01_HostingAndStartup.Services;

var builder = WebApplication.CreateBuilder(args);

// --- Assembly points on WebApplicationBuilder (Services / Configuration / Logging / Environment / Host / WebHost) ---
builder.Services.AddSingleton<StartupProbe>();
builder.Services.AddScoped<ScopedWorkItem>();
builder.Services.AddHostedService<CampusHeartbeatWorker>();
builder.Services.AddHostedService<LifetimeEventsLogger>();

builder.Services.Configure<HostOptions>(options =>
{
    // Keep graceful shutdown snappy for lab demos; production often 30s+
    options.ShutdownTimeout = TimeSpan.FromSeconds(15);
    // Default is StopHost — unhandled exceptions in BackgroundService stop the process
    options.BackgroundServiceExceptionBehavior = BackgroundServiceExceptionBehavior.StopHost;
});

// Optional: customize listen URLs (launchSettings usually wins in local `dotnet run`)
// builder.WebHost.UseUrls("http://localhost:5101");

var app = builder.Build();

// --- After Build: WebApplication is IHost + IApplicationBuilder + IEndpointRouteBuilder ---
app.Logger.LogInformation(
    "Env={Env}, ContentRoot={ContentRoot}, WebRoot={WebRoot}, ApplicationName={App}",
    app.Environment.EnvironmentName,
    app.Environment.ContentRootPath,
    app.Environment.WebRootPath,
    app.Environment.ApplicationName);

// Static files from Web root (wwwroot) — demonstrates ContentRoot vs WebRoot
app.UseStaticFiles();

// Layered configuration demo: appsettings.json < appsettings.{Environment}.json
app.MapGet("/", (IConfiguration config, IWebHostEnvironment env) => Results.Ok(new
{
    message = config["Greeting"] ?? "Hello",
    campus = config["Campus:Name"],
    environment = env.EnvironmentName,
    contentRoot = env.ContentRootPath,
    webRoot = env.WebRootPath,
    note = "DOTNET_ENVIRONMENT takes precedence over ASPNETCORE_ENVIRONMENT when using WebApplication."
}));

app.MapGet("/host-info", (IWebHostEnvironment env, IConfiguration config) => Results.Ok(new
{
    env.EnvironmentName,
    env.ApplicationName,
    env.ContentRootPath,
    env.WebRootPath,
    isDevelopment = env.IsDevelopment(),
    isProduction = env.IsProduction(),
    isStaging = env.IsStaging(),
    greeting = config["Greeting"],
    heartbeatSeconds = config.GetValue("Heartbeat:IntervalSeconds", 10)
}));

// Minimal student-facing ping (real domain flavor, no DB in step 1)
app.MapGet("/students/ping", () => Results.Ok(new
{
    ok = true,
    sample = new { studentNumber = "2024001001", fullName = "张三", major = "计算机科学" }
}));

app.Run();

// Expose for WebApplicationFactory integration tests (step 9 will deepen this pattern)
public partial class Program;
