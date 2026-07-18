using Microsoft.AspNetCore.Diagnostics;
using Serilog;
using Step08_LoggingErrorsHealth;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((ctx, services, cfg) =>
    {
        cfg.ReadFrom.Configuration(ctx.Configuration)
            .ReadFrom.Services(services)
            .Enrich.FromLogContext()
            .WriteTo.Console();

        // Seq is optional for local/CI; only wire when a real URL is configured
        var seqUrl = ctx.Configuration["Seq:ServerUrl"];
        if (!string.IsNullOrWhiteSpace(seqUrl)
            && !seqUrl.Contains("127.0.0.1:1", StringComparison.Ordinal)
            && Uri.TryCreate(seqUrl, UriKind.Absolute, out _))
        {
            cfg.WriteTo.Seq(seqUrl);
        }
    });

    builder.Services.AddProblemDetails();
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddHealthChecks()
        .AddCheck("self", () => Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy(), tags: ["live"])
        .AddNpgSql(
            builder.Configuration.GetConnectionString("Postgres")
            ?? "Host=localhost;Port=5432;Database=postgres;Username=dotnet;Password=dotnet_dev",
            name: "postgres",
            tags: ["ready"])
        .AddRedis(
            builder.Configuration.GetConnectionString("Redis") ?? "localhost:6380",
            name: "redis",
            tags: ["ready"]);

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();
    app.Use(async (ctx, next) =>
    {
        var cid = ctx.Request.Headers.TryGetValue("X-Correlation-Id", out var v) && !string.IsNullOrWhiteSpace(v)
            ? v.ToString()
            : Guid.NewGuid().ToString("N");
        ctx.Response.OnStarting(() =>
        {
            ctx.Response.Headers["X-Correlation-Id"] = cid;
            return Task.CompletedTask;
        });
        using (Serilog.Context.LogContext.PushProperty("CorrelationId", cid))
        {
            await next();
        }
    });

    app.MapGet("/", () => Results.Ok(new { lab = "Step08 Logging / Errors / Health", seq = "http://localhost:5341" }));
    app.MapGet("/api/boom", (HttpContext _) =>
{
    throw new InvalidOperationException("step08 boom");
});

    app.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("live")
    });
    app.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
    {
        Predicate = r => r.Tags.Contains("ready")
    });

    app.Run();
}
finally
{
    Log.CloseAndFlush();
}

public partial class Program;

namespace Step08_LoggingErrorsHealth
{
    public sealed class GlobalExceptionHandler(IProblemDetailsService pds) : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await pds.WriteAsync(new ProblemDetailsContext
            {
                HttpContext = httpContext,
                ProblemDetails =
                {
                    Title = "Unhandled error",
                    Detail = exception.Message,
                    Status = 500
                }
            });
            return true;
        }
    }
}
