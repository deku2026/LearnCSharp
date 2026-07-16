// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : DependencyInjectionBasics
// Topic id : stage09/section06/dependency_injection_basics
//
// 步骤 1：IoC/DI — Microsoft.Extensions.DependencyInjection (ServiceCollection)

using System.Diagnostics;
using LearnCSharp.Topics;
using Microsoft.Extensions.DependencyInjection;

namespace LearnCSharp.Stage09.Section06;

internal static class DependencyInjectionBasics
{
    private interface IEmailSender
    {
        string Send(string to, string body);
    }

    private sealed class SmtpEmailSender : IEmailSender
    {
        public string Send(string to, string body) => $"SMTP→{to}:{body}";
    }

    private sealed class FakeEmailSender : IEmailSender
    {
        public string Send(string to, string body) => $"FAKE→{to}:{body}";
    }

    private sealed class OrderService(IEmailSender email)
    {
        public string Place(string to) => email.Send(to, "order-ok");
    }

    [LearnTopic("stage09/section06/dependency_injection_basics")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== DependencyInjectionBasics ===");
        DemoWithoutDiVsWithDi();
        DemoSwapImplementation();
        return 0;
    }

    private static void DemoWithoutDiVsWithDi()
    {
        Console.WriteLine("-- ctor injection vs hard-coded new --");
        var coupled = new OrderService(new SmtpEmailSender());
        Debug.Assert(coupled.Place("a@b.c").StartsWith("SMTP", StringComparison.Ordinal));

        var services = new ServiceCollection();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddTransient<OrderService>();
        using ServiceProvider provider = services.BuildServiceProvider();
        OrderService svc = provider.GetRequiredService<OrderService>();
        string result = svc.Place("ada@example.com");
        Debug.Assert(result.Contains("order-ok", StringComparison.Ordinal));
        Console.WriteLine($"  ServiceCollection → {result}");
        Console.WriteLine("  APIs: AddSingleton/AddTransient + BuildServiceProvider + GetRequiredService");
    }

    private static void DemoSwapImplementation()
    {
        Console.WriteLine("-- swap implementation for tests --");
        var services = new ServiceCollection();
        services.AddSingleton<IEmailSender, FakeEmailSender>();
        services.AddTransient<OrderService>();
        using ServiceProvider provider = services.BuildServiceProvider();
        string result = provider.GetRequiredService<OrderService>().Place("t@est");
        Debug.Assert(result.StartsWith("FAKE", StringComparison.Ordinal));
        Console.WriteLine($"  test double: {result}");
    }
}
