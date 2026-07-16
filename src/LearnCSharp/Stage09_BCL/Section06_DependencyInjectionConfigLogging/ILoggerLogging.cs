// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : ILoggerLogging
// Topic id : stage09/section06/ilogger_logging
//
// 步骤 4：日志级别 + 结构化消息（BCL 教育 ILogger，无 Microsoft.Extensions.Logging 包）

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section06;

internal static class ILoggerLogging
{
    private enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }

    private interface IMiniLogger
    {
        void Log(LogLevel level, string message, params object?[] args);
        bool IsEnabled(LogLevel level);
    }

    private interface IMiniLogger<TCategory> : IMiniLogger;

    private sealed class ConsoleLogger<T>(LogLevel minLevel) : IMiniLogger<T>
    {
        public bool IsEnabled(LogLevel level) => level >= minLevel && level != LogLevel.None;

        public void Log(LogLevel level, string message, params object?[] args)
        {
            if (!IsEnabled(level)) return;
            // structured-ish: keep template + args (real ILogger uses message templates)
            string rendered = args.Length == 0 ? message : string.Format(System.Globalization.CultureInfo.InvariantCulture, message, args);
            Console.WriteLine($"[{level}] {typeof(T).Name}: {rendered}");
        }
    }

    private sealed class CheckoutService(IMiniLogger<CheckoutService> logger)
    {
        public void Checkout(int orderId, decimal amount)
        {
            logger.Log(LogLevel.Information, "Checkout started for order {0} amount {1}", orderId, amount);
            if (amount < 0)
            {
                logger.Log(LogLevel.Error, "Invalid amount {0} for order {1}", amount, orderId);
                return;
            }
            logger.Log(LogLevel.Debug, "Order {0} validated", orderId);
        }
    }

    [LearnTopic("stage09/section06/ilogger_logging")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ILoggerLogging ===");
        DemoLevelsAndFiltering();
        DemoStructuredStyle();
        return 0;
    }

    private static void DemoLevelsAndFiltering()
    {
        Console.WriteLine("-- levels + min-level filter --");
        IMiniLogger<CheckoutService> verbose = new ConsoleLogger<CheckoutService>(LogLevel.Debug);
        IMiniLogger<CheckoutService> quiet = new ConsoleLogger<CheckoutService>(LogLevel.Warning);
        Debug.Assert(verbose.IsEnabled(LogLevel.Debug));
        Debug.Assert(!quiet.IsEnabled(LogLevel.Information));
        verbose.Log(LogLevel.Debug, "debug visible");
        quiet.Log(LogLevel.Information, "info hidden");
        quiet.Log(LogLevel.Warning, "warning visible");
        Console.WriteLine("  map: Trace/Debug/Information/Warning/Error/Critical");
    }

    private static void DemoStructuredStyle()
    {
        Console.WriteLine("-- category logger + template args (structured logging idea) --");
        var logger = new ConsoleLogger<CheckoutService>(LogLevel.Information);
        var svc = new CheckoutService(logger);
        svc.Checkout(1001, 19.99m);
        svc.Checkout(1002, -1m);
        Console.WriteLine("  real ILogger: LogInformation(\"Order {OrderId}\", id) → structured fields");
        Console.WriteLine("  providers: Console, Debug, EventSource, OpenTelemetry, Serilog sinks");
        Debug.Assert(true);
    }
}
