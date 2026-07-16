// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第2部分-异常实践模式与诊断.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section02_ExceptionPracticeAndDiagnostics
// Item     : DiagnosticsStackTraceLoggingActivity
// Topic id : stage06/section02/diagnostics_stacktrace_logging_activity
//
// 步骤 4：StackTrace、结构化日志消息模板、Activity 分布式追踪
// 无第三方包：EventSource + Activity（BCL）演示结构化/可关联诊断

using System.Diagnostics;
using System.Diagnostics.Tracing;
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
        DemoStructuredLogTemplateAndEventSource();
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

    private static void DemoStructuredLogTemplateAndEventSource()
    {
        Console.WriteLine("-- structured log (no packages): message template + EventSource --");
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
            string template = "process user {UserId} order {OrderId} failed";
            // Template kept separate from values (structured); rendering is only for console demo.
            string rendered = template
                .Replace("{UserId}", userId.ToString(), StringComparison.Ordinal)
                .Replace("{OrderId}", orderId.ToString(), StringComparison.Ordinal);

            Debug.Assert(rendered.Contains("42") && rendered.Contains("1001"));
            Console.WriteLine($"  template : {template}");
            Console.WriteLine($"  rendered : {rendered}");
            Console.WriteLine($"  exception: {ex.GetType().Name} — always pass full ex object to logger");
            Console.WriteLine("  BAD:  $\"user {userId}\"  (not structured)");
            Console.WriteLine("  GOOD: \"user {UserId}\", userId");

            // BCL EventSource: structured payloads without Microsoft.Extensions.Logging.
            using var listener = new DemoEventListener();
            DemoOrderEventSource.Log.OrderFailed(userId, orderId, ex.GetType().Name);
            Debug.Assert(listener.LastPayload is not null);
            Debug.Assert(listener.LastPayload.Contains("42", StringComparison.Ordinal));
            Debug.Assert(listener.LastPayload.Contains("1001", StringComparison.Ordinal));
            Console.WriteLine($"  EventSource payload: {listener.LastPayload}");
            Console.WriteLine("  note: production apps usually use ILogger + OpenTelemetry exporters");
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
            // Correlate logs: include TraceId in structured fields (same idea as EventSource payload).
            activity.AddEvent(new ActivityEvent(
                "order.validation",
                tags: new ActivityTagsCollection { { "result", "ok" } }));
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
        Console.WriteLine("  structured log field + Activity.TraceId = request-wide correlation without packages");
    }

    [EventSource(Name = "LearnCSharp-Stage06-Orders")]
    private sealed class DemoOrderEventSource : EventSource
    {
        public static readonly DemoOrderEventSource Log = new();

        private DemoOrderEventSource() { }

        [Event(1, Level = EventLevel.Error, Message = "Order failed user={0} order={1} ex={2}")]
        public void OrderFailed(int userId, int orderId, string exceptionType) =>
            WriteEvent(1, userId, orderId, exceptionType);
    }

    private sealed class DemoEventListener : EventListener
    {
        public string? LastPayload { get; private set; }

        protected override void OnEventSourceCreated(EventSource eventSource)
        {
            if (eventSource.Name == "LearnCSharp-Stage06-Orders")
                EnableEvents(eventSource, EventLevel.Informational);
        }

        protected override void OnEventWritten(EventWrittenEventArgs eventData)
        {
            if (eventData.EventName == "OrderFailed" && eventData.Payload is { Count: > 0 })
                LastPayload = string.Join(',', eventData.Payload.Select(static p => p?.ToString() ?? ""));
        }
    }
}
