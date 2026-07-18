using System.Collections.Concurrent;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Part08_02_Troubleshooting;

Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog((ctx, cfg) => cfg
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.Seq(ctx.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

builder.Services.AddSingleton<FaultSwitch>();
builder.Services.AddSingleton<LatencyHistogram>();

// Self-call base URL comes from launchSettings / config (tests override via handler)
builder.Services.AddHttpClient("downstream", (sp, c) =>
{
    var baseUrl = sp.GetRequiredService<IConfiguration>()["Downstream:BaseUrl"] ?? "http://127.0.0.1:5703";
    c.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    c.Timeout = TimeSpan.FromSeconds(10);
}).AddStandardResilienceHandler();

builder.Services.AddOpenTelemetry()
    .ConfigureResource(r => r.AddService("Part08_02_Troubleshooting"))
    .WithTracing(t => t.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddSource("Campus.Troubleshoot").AddOtlpExporter())
    .WithMetrics(m => m.AddAspNetCoreInstrumentation().AddHttpClientInstrumentation().AddMeter("Campus.Troubleshoot").AddOtlpExporter());

var app = builder.Build();

app.Use(async (ctx, next) =>
{
    var cid = ctx.Request.Headers.TryGetValue("X-Correlation-Id", out var v) && !string.IsNullOrWhiteSpace(v)
        ? v.ToString()
        : Activity.Current?.TraceId.ToString() ?? Guid.NewGuid().ToString("N");
    ctx.Response.OnStarting(() =>
    {
        ctx.Response.Headers["X-Correlation-Id"] = cid;
        return Task.CompletedTask;
    });
    using (Serilog.Context.LogContext.PushProperty("CorrelationId", cid))
    using (Serilog.Context.LogContext.PushProperty("TraceId", Activity.Current?.TraceId.ToString()))
    {
        await next();
    }
});

app.MapGet("/", () => Results.Ok(new
{
    lab = "Part08_02 Troubleshooting",
    runbook = new[]
    {
        "1 Metrics → GET /diag/metrics (avg/max latency, fail counters)",
        "2 Traces → Aspire Dashboard http://localhost:18888",
        "3 Logs → Seq http://localhost:5341 filter CorrelationId",
        "4 Inject slow/fail → POST /diag/fault",
        "5 Reproduce → GET /api/orders/checkout",
        "6 Fingerprint → slow+low CPU = dependency; 503 burst = downstream down"
    }
}));

app.MapGet("/sim/downstream", async (FaultSwitch faults, LatencyHistogram hist, ILogger<Program> log) =>
{
    var sw = Stopwatch.StartNew();
    try
    {
        if (faults.DelayMs > 0)
        {
            await Task.Delay(faults.DelayMs);
        }

        if (faults.TryFail())
        {
            log.LogWarning("Downstream injected failure, remaining={Remaining}", faults.FailCount);
            return Results.StatusCode(StatusCodes.Status503ServiceUnavailable);
        }

        return Results.Ok(new { ok = true, service = "downstream-sim" });
    }
    finally
    {
        sw.Stop();
        hist.Record(sw.Elapsed.TotalMilliseconds);
    }
});

app.MapPost("/diag/fault", (FaultRequest req, FaultSwitch faults) =>
{
    faults.Configure(req.DelayMs, req.FailCount);
    return Results.Ok(new { faults.DelayMs, faults.FailCount });
});

app.MapGet("/diag/metrics", (LatencyHistogram hist, FaultSwitch faults) => Results.Ok(new
{
    samples = hist.Count,
    avgMs = hist.Average,
    maxMs = hist.Max,
    fault = new { faults.DelayMs, remainingFails = faults.FailCount },
    counters = new
    {
        success = TroubleshootMetrics.SuccessSnapshot,
        fail = TroubleshootMetrics.FailSnapshot
    },
    fingerprintHints = new
    {
        slowDownstream = "high maxMs + CPU not pegged",
        dependencyDown = "checkout 503 + fail counter up",
        threadPoolStarvation = "use dotnet-counters: ThreadPool Queue Length rising"
    }
}));

app.MapGet("/api/orders/checkout", async (IHttpClientFactory http, ILogger<Program> log) =>
{
    using var activity = TroubleshootActivity.Source.StartActivity("checkout");
    log.LogInformation("Checkout started");
    var client = http.CreateClient("downstream");
    try
    {
        using var res = await client.GetAsync("sim/downstream");
        if (!res.IsSuccessStatusCode)
        {
            TroubleshootMetrics.RecordFail();
            log.LogError("Checkout failed dependency status={Status}", (int)res.StatusCode);
            return Results.Problem(
                title: "Downstream failed",
                detail: $"status={(int)res.StatusCode}",
                statusCode: StatusCodes.Status503ServiceUnavailable);
        }

        TroubleshootMetrics.RecordSuccess();
        log.LogInformation("Checkout succeeded");
        return Results.Ok(new { orderId = Guid.NewGuid(), status = "placed" });
    }
    catch (Exception ex)
    {
        TroubleshootMetrics.RecordFail();
        log.LogError(ex, "Checkout exception (timeout/circuit?)");
        return Results.Problem(title: "Checkout error", detail: ex.Message, statusCode: 503);
    }
});

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

app.Run();

public partial class Program;

namespace Part08_02_Troubleshooting
{
    public sealed record FaultRequest(int DelayMs, int FailCount);

    public sealed class FaultSwitch
    {
        private int _fail;

        public int DelayMs { get; private set; }
        public int FailCount => Volatile.Read(ref _fail);

        public void Configure(int delayMs, int failCount)
        {
            DelayMs = Math.Clamp(delayMs, 0, 10_000);
            Interlocked.Exchange(ref _fail, Math.Clamp(failCount, 0, 100));
        }

        public bool TryFail()
        {
            while (true)
            {
                var current = Volatile.Read(ref _fail);
                if (current <= 0)
                {
                    return false;
                }

                if (Interlocked.CompareExchange(ref _fail, current - 1, current) == current)
                {
                    return true;
                }
            }
        }
    }

    public sealed class LatencyHistogram
    {
        private readonly ConcurrentBag<double> _samples = [];

        public void Record(double ms) => _samples.Add(ms);
        public int Count => _samples.Count;
        public double Average => _samples.IsEmpty ? 0 : _samples.Average();
        public double Max => _samples.IsEmpty ? 0 : _samples.Max();
    }

    public static class TroubleshootActivity
    {
        public static readonly ActivitySource Source = new("Campus.Troubleshoot");
    }

    public static class TroubleshootMetrics
    {
        private static long _success;
        private static long _fail;
        private static readonly Meter Meter = new("Campus.Troubleshoot");
        private static readonly Counter<long> Success = Meter.CreateCounter<long>("checkout.success");
        private static readonly Counter<long> Fail = Meter.CreateCounter<long>("checkout.fail");

        public static long SuccessSnapshot => Interlocked.Read(ref _success);
        public static long FailSnapshot => Interlocked.Read(ref _fail);

        public static void RecordSuccess()
        {
            Interlocked.Increment(ref _success);
            Success.Add(1);
        }

        public static void RecordFail()
        {
            Interlocked.Increment(ref _fail);
            Fail.Add(1);
        }
    }
}
