using Microsoft.Extensions.Options;
using Step02_DiConfigOptions.Options;
using Step02_DiConfigOptions.Services;

var builder = WebApplication.CreateBuilder(args);

// Development already validates scopes; keep explicit for the lab clarity
builder.Host.UseDefaultServiceProvider(options =>
{
    options.ValidateScopes = builder.Environment.IsDevelopment();
    options.ValidateOnBuild = builder.Environment.IsDevelopment();
});

// --- A3 lifetimes ---
builder.Services.AddTransient<IOperationTransient, Operation>();
builder.Services.AddScoped<IOperationScoped, Operation>();
builder.Services.AddSingleton<IOperationSingleton, Operation>();

// --- A5 scoped stand-in for DbContext ---
builder.Services.AddScoped<FakeDbContext>();
builder.Services.AddHostedService<ScopedSafeWorker>();

// --- A8 multi-implementation ---
builder.Services.AddSingleton<IGuidWriter, ConsoleGuidWriter>();
builder.Services.AddSingleton<IGuidWriter, FileGuidWriter>();

// --- A9 keyed services ---
builder.Services.AddKeyedSingleton<IPaymentGateway, AlipayGateway>("alipay");
builder.Services.AddKeyedSingleton<IPaymentGateway, WeChatPayGateway>("wechat");

// --- A11 Scrutor decorator ---
builder.Services.AddSingleton<IStudentDirectory, InMemoryStudentDirectory>();
builder.Services.Decorate<IStudentDirectory, LoggingStudentDirectory>();

// --- C Options: bind + data annotations + fail-fast on start ---
builder.Services
    .AddOptions<CampusShopOptions>()
    .Bind(builder.Configuration.GetSection(CampusShopOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

var app = builder.Build();

app.MapGet("/", () => Results.Ok(new
{
    lab = "Step02 DI / Configuration / Options",
    endpoints = new[]
    {
        "GET /di/lifetimes",
        "GET /di/writers",
        "GET /di/pay/{gateway}",
        "GET /di/students",
        "GET /config/greeting",
        "GET /options/shop",
        "GET /options/shop-monitor"
    }
}));

// Prove Transient vs Scoped vs Singleton instance reuse within one request and across requests
app.MapGet("/di/lifetimes", (
    IOperationTransient t1,
    IOperationTransient t2,
    IOperationScoped s1,
    IOperationScoped s2,
    IOperationSingleton single) =>
{
    return Results.Ok(new
    {
        transient = new { first = t1.OperationId, second = t2.OperationId, same = t1.OperationId == t2.OperationId },
        scoped = new { first = s1.OperationId, second = s2.OperationId, same = s1.OperationId == s2.OperationId },
        singleton = single.OperationId,
        note = "Within one request: Transient differs; Scoped same; Singleton same across requests."
    });
});

app.MapGet("/di/writers", (IEnumerable<IGuidWriter> writers) =>
    Results.Ok(writers.Select(w => new { w.Name, sample = w.Write() })));

app.MapGet("/di/pay/{gateway}", (string gateway, IServiceProvider sp) =>
{
    var payment = sp.GetRequiredKeyedService<IPaymentGateway>(gateway);
    return Results.Ok(new { gateway, result = payment.Charge(19.9m) });
});

app.MapGet("/di/students", (IStudentDirectory directory) => Results.Ok(directory.ListNames()));

// Configuration: indexer + layered sources (appsettings → env-specific → env vars → cmdline)
app.MapGet("/config/greeting", (IConfiguration config) => Results.Ok(new
{
    greeting = config["Greeting"],
    campusName = config["CampusShop:ShopName"],
    doubleUnderscoreHint = "Env var CampusShop__SmtpHost maps to CampusShop:SmtpHost"
}));

// IOptions is fixed after first resolve (singleton); IOptionsMonitor can reload on file change
app.MapGet("/options/shop", (IOptions<CampusShopOptions> options) => Results.Ok(options.Value));
app.MapGet("/options/shop-monitor", (IOptionsMonitor<CampusShopOptions> monitor) => Results.Ok(monitor.CurrentValue));

// Snapshot is scoped — same within request, may change next request after reload
app.MapGet("/options/shop-snapshot", (IOptionsSnapshot<CampusShopOptions> snapshot) => Results.Ok(snapshot.Value));

app.Run();

public partial class Program;
