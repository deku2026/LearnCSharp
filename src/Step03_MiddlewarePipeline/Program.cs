using System.Diagnostics;
using Step03_MiddlewarePipeline.Middleware;
using Step03_MiddlewarePipeline.Services;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddScoped<RequestIdFactory>();
builder.Services.AddScoped<ScopedRequestCounter>();
builder.Services.AddScoped<RequestTimingMiddleware>(); // IMiddleware → per-request activation

var app = builder.Build();

// Order iron law (outermost first): exception → timing → api-key short-circuit → branch → authz demo → endpoints
app.UseMiddleware<GlobalExceptionMiddleware>();

// Convention middleware (singleton): use method injection for scoped services
app.UseRequestTimingConvention();

// IMiddleware (scoped each request) — can inject scoped services in ctor
app.UseMiddleware<RequestTimingMiddleware>();

// Short-circuit without calling next if missing API key (except public paths)
app.Use(async (context, next) =>
{
    if (context.Request.Path.StartsWithSegments("/public")
        || context.Request.Path.StartsWithSegments("/branch")
        || context.Request.Path == "/favicon.ico"
        || context.Request.Path == "/robots.txt")
    {
        await next(context);
        return;
    }

    if (!context.Request.Headers.ContainsKey("X-Api-Key"))
    {
        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        await context.Response.WriteAsync("Missing X-Api-Key");
        return; // short-circuit
    }

    await next(context);
});

// MapShortCircuit: no middleware pipeline for these paths after registration point... 
// Actually MapShortCircuit endpoints skip remaining middleware when matched early.
// Document: short-circuit endpoints bypass remaining middleware when configured.
app.MapShortCircuit(404, "robots.txt", "favicon.ico");

// UseWhen rejoins main pipeline; MapWhen does not
app.UseWhen(
    ctx => ctx.Request.Path.StartsWithSegments("/branch"),
    branch =>
    {
        branch.Use(async (ctx, next) =>
        {
            ctx.Response.Headers["X-Branch"] = "use-when";
            await next(ctx);
        });
    });

// Demo wrong order would be UseAuthorization before UseAuthentication — we document, not enable broken auth here.
// Response OnStarting: set header before body starts
app.Use(async (context, next) =>
{
    context.Response.OnStarting(() =>
    {
        context.Response.Headers["X-On-Starting"] = "1";
        return Task.CompletedTask;
    });
    await next(context);
});

app.MapGet("/public/hello", () => Results.Ok(new { ok = true, area = "public" }));

app.MapGet("/secure/ping", (HttpContext http, ScopedRequestCounter counter) =>
{
    counter.Increment();
    return Results.Ok(new
    {
        ok = true,
        path = http.Request.Path.Value,
        itemsRequestId = http.Items["RequestId"],
        count = counter.Value
    });
});

app.MapGet("/branch/info", () => Results.Ok(new { branch = true }));

app.MapGet("/boom", (HttpContext _) =>
{
    throw new InvalidOperationException("boom for global exception middleware");
});

app.MapGet("/response-started-demo", async (HttpContext ctx) =>
{
    await ctx.Response.WriteAsync("partial-");
    // After body started, changing status would throw — lab documents HasStarted
    return Results.Empty;
});

app.Run();

public partial class Program;
