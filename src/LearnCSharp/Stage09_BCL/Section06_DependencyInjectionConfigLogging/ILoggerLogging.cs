// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : ILoggerLogging
// Topic id : stage09/section06/ilogger_logging
//
// 步骤 4：ILogger + 日志级别 + 结构化消息模板（Microsoft.Extensions.Logging）

using System.Diagnostics;
using LearnCSharp.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Stage09.Section06;

internal static class ILoggerLogging
{
    private sealed class CheckoutService(ILogger<CheckoutService> logger)
    {
        public void Checkout(int orderId, decimal amount)
        {
            logger.LogInformation("Checkout started for order {OrderId} amount {Amount}", orderId, amount);
            if (amount < 0)
            {
                logger.LogError("Invalid amount {Amount} for order {OrderId}", amount, orderId);
                return;
            }

            logger.LogDebug("Order {OrderId} validated", orderId);
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
        Console.WriteLine("-- levels + min-level filter (console provider) --");
        using ServiceProvider provider = new ServiceCollection()
            .AddLogging(b =>
            {
                b.ClearProviders();
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Warning);
            })
            .BuildServiceProvider();

        ILoggerFactory factory = provider.GetRequiredService<ILoggerFactory>();
        ILogger logger = factory.CreateLogger("Demo.Filter");
        Debug.Assert(logger.IsEnabled(LogLevel.Warning));
        Debug.Assert(!logger.IsEnabled(LogLevel.Information));
        logger.LogInformation("info hidden by min level");
        logger.LogWarning("warning visible");
        Console.WriteLine("  map: Trace/Debug/Information/Warning/Error/Critical");
    }

    private static void DemoStructuredStyle()
    {
        Console.WriteLine("-- category logger + message templates --");
        using ServiceProvider provider = new ServiceCollection()
            .AddLogging(b =>
            {
                b.ClearProviders();
                b.AddConsole();
                b.SetMinimumLevel(LogLevel.Information);
            })
            .AddTransient<CheckoutService>()
            .BuildServiceProvider();

        CheckoutService svc = provider.GetRequiredService<CheckoutService>();
        svc.Checkout(1001, 19.99m);
        svc.Checkout(1002, -1m);
        Console.WriteLine("  LogInformation(\"Order {OrderId}\", id) → structured fields for sinks");
        Debug.Assert(svc is not null);
    }
}
