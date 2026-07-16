// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第2部分-异常实践模式与诊断.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section02_ExceptionPracticeAndDiagnostics
// Item     : DiagnosticsStackTraceLoggingActivity
// Topic id : stage06/section02/diagnostics_stacktrace_logging_activity
//
// 步骤 4：StackTrace、结构化日志消息模板、Activity 分布式追踪

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section02;

internal static class DiagnosticsStackTraceLoggingActivity
{
    private const string OrderSourceName = "LearnCSharp.Stage06.Orders";

    [LearnTopic("stage06/section02/diagnostics_stacktrace_logging_activity")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DiagnosticsStackTraceLoggingActivity ===");
        DemoStackTraceDepth();
        DemoStructuredLogTemplate();
        DemoActivityTrace();
        return 0;
    }

    private static void DemoStackTraceDepth()
    {
        Console.WriteLine("-- Message vs StackTrace vs ToString --");
        try
        {
            LayerA();
        }
        catch (Exception ex)
        {
            Debug.Assert(!string.IsNullOrEmpty(ex.Message));
            Debug.Assert(ex.StackTrace is not null);
            string full = ex.ToString();
            Debug.Assert(full.Contains(ex.Message, StringComparison.Ordinal));
            Debug.Assert(full.Contains(nameof(LayerC), StringComparison.Ordinal));

            var st = new StackTrace(ex, fNeedFileInfo: false);
            Debug.Assert(st.FrameCount > 0);

            Console.WriteLine($"  Message   : {ex.Message}");
            Console.WriteLine($"  frames    : {st.FrameCount}");
            Console.WriteLine($"  ToString  includes type+message+stack: {full.Contains(nameof(InvalidOperationException))}");
            Console.WriteLine($"  prefer Log(ex) / ToString over Message-only");
        }
    }

    private static void LayerA() => LayerB();
    private static void LayerB() => LayerC();
    private static void LayerC() => throw new InvalidOperationException("pipeline failed");

    private static void DemoStructuredLogTemplate()
    {
        Console.WriteLine("-- structured log: template + named placeholders (not string interpolation) --");
        // ILogger style: LogError(ex, "process user {UserId} order {OrderId}", userId, orderId)
        // Named placeholders become queryable fields in Seq/ELK/Azure Monitor.
        int userId = 42;
        int orderId = 1001;
        try
        {
            throw new IOException("disk full");
        }
        catch (Exception ex)
        {
            // Demo template formatting without Microsoft.Extensions.Logging package.
            string template = "process user {UserId} order {OrderId} failed";
            string rendered = template
                .Replace("{UserId}", userId.ToString(), StringComparison.Ordinal)
                .Replace("{OrderId}", orderId.ToString(), StringComparison.Ordinal);

            Debug.Assert(rendered.Contains("42") && rendered.Contains("1001"));
            Console.WriteLine($"  template : {template}");
            Console.WriteLine($"  rendered : {rendered}");
            Console.WriteLine($"  exception: {ex.GetType().Name} — always pass full ex object to logger");
            Console.WriteLine("  BAD:  $\"user {userId}\"  (not structured)");
            Console.WriteLine("  GOOD: \"user {UserId}\", userId");
        }
    }

    private static void DemoActivityTrace()
    {
        Console.WriteLine("-- Activity / distributed tracing (OpenTelemetry base) --");
        using ActivityListener listener = new()
        {
            ShouldListenTo = static source => source.Name == OrderSourceName,
            Sample = static (ref ActivityCreationOptions<ActivityContext> _) => ActivitySamplingResult.AllDataAndRecorded,
        };
        ActivitySource.AddActivityListener(listener);

        using ActivitySource orderSource = new(OrderSourceName);
        string? traceId;
        using (Activity? activity = orderSource.StartActivity("ProcessOrder"))
        {
            Debug.Assert(activity is not null);
            activity.SetTag("order.id", 1001);
            activity.SetTag("user.id", 42);
            traceId = activity.TraceId.ToString();

            using (Activity? child = orderSource.StartActivity("ChargePayment"))
            {
                Debug.Assert(child is not null);
                Debug.Assert(child.TraceId == activity.TraceId);
                child.SetTag("payment.provider", "demo");
                Console.WriteLine($"  parent={activity.OperationName}, child={child.OperationName}");
                Console.WriteLine($"  same TraceId: {child.TraceId == activity.TraceId}");
            }

            activity.SetStatus(ActivityStatusCode.Ok);
        }

        Debug.Assert(!string.IsNullOrEmpty(traceId));
        Console.WriteLine($"  TraceId={traceId} correlates logs across services");
    }
}
