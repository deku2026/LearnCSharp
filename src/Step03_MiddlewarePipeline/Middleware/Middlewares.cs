using System.Diagnostics;
using Step03_MiddlewarePipeline.Services;

namespace Step03_MiddlewarePipeline.Middleware;

/// <summary>Outermost: catches unhandled exceptions → Problem-like JSON.</summary>
public sealed class GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            if (context.Response.HasStarted)
            {
                throw;
            }

            context.Response.Clear();
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new
            {
                title = "Unhandled exception",
                detail = ex.Message,
                traceId = context.TraceIdentifier
            });
        }
    }
}

/// <summary>Convention middleware (activated as singleton) — inject scoped via method parameter.</summary>
public sealed class RequestTimingConventionMiddleware(RequestDelegate next)
{
    public async Task InvokeAsync(HttpContext context, RequestIdFactory idFactory, ILogger<RequestTimingConventionMiddleware> logger)
    {
        var sw = Stopwatch.StartNew();
        var id = idFactory.Create();
        context.Items["RequestId"] = id;
        logger.LogInformation("MW convention IN {Id} {Path}", id, context.Request.Path);
        context.Response.OnStarting(() =>
        {
            sw.Stop();
            context.Response.Headers["X-Request-Id"] = id;
            context.Response.Headers["X-Elapsed-Ms"] = sw.ElapsedMilliseconds.ToString();
            return Task.CompletedTask;
        });
        await next(context);
        logger.LogInformation("MW convention OUT {Id} {Elapsed}ms", id, sw.ElapsedMilliseconds);
    }
}

public static class RequestTimingConventionExtensions
{
    public static IApplicationBuilder UseRequestTimingConvention(this IApplicationBuilder app)
        => app.UseMiddleware<RequestTimingConventionMiddleware>();
}

/// <summary>IMiddleware: resolved from DI each request → safe to inject scoped services in ctor.</summary>
public sealed class RequestTimingMiddleware(
    ScopedRequestCounter counter,
    ILogger<RequestTimingMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        counter.Increment();
        logger.LogInformation("IMiddleware IN count={Count}", counter.Value);
        context.Response.OnStarting(() =>
        {
            context.Response.Headers["X-Scoped-Count"] = counter.Value.ToString();
            return Task.CompletedTask;
        });
        await next(context);
        logger.LogInformation("IMiddleware OUT count={Count}", counter.Value);
    }
}
