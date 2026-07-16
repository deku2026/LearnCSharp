// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : DependencyInjectionBasics
// Topic id : stage09/section06/dependency_injection_basics
//
// 步骤 1：IoC/DI 概念 — BCL 教育容器（无 Microsoft.Extensions.DependencyInjection 包）

using System.Diagnostics;
using LearnCSharp.Topics;

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

    /// <summary>Tiny educational DI: register interface→impl, resolve ctor graph.</summary>
    private sealed class MiniContainer
    {
        private readonly Dictionary<Type, Func<object>> _map = [];

        public void AddSingleton<TService, TImpl>() where TImpl : TService, new()
        {
            TService? instance = default;
            _map[typeof(TService)] = () => instance ??= new TImpl();
        }

        public void AddSingleton<TService>(TService instance) where TService : notnull
            => _map[typeof(TService)] = () => instance;

        public void AddTransient<TService>(Func<MiniContainer, TService> factory) where TService : notnull
            => _map[typeof(TService)] = () => factory(this)!;

        public T GetRequiredService<T>() where T : notnull
        {
            if (!_map.TryGetValue(typeof(T), out Func<object>? factory))
                throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
            return (T)factory();
        }
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
        // without DI: tight coupling
        var coupled = new OrderService(new SmtpEmailSender());
        Debug.Assert(coupled.Place("a@b.c").StartsWith("SMTP", StringComparison.Ordinal));

        var services = new MiniContainer();
        services.AddSingleton<IEmailSender, SmtpEmailSender>();
        services.AddTransient(c => new OrderService(c.GetRequiredService<IEmailSender>()));
        OrderService svc = services.GetRequiredService<OrderService>();
        string result = svc.Place("ada@example.com");
        Debug.Assert(result.Contains("order-ok", StringComparison.Ordinal));
        Console.WriteLine($"  resolved OrderService → {result}");
        Console.WriteLine("  real apps: ServiceCollection + BuildServiceProvider / Host");
    }

    private static void DemoSwapImplementation()
    {
        Console.WriteLine("-- swap implementation for tests --");
        var services = new MiniContainer();
        services.AddSingleton<IEmailSender, FakeEmailSender>();
        services.AddTransient(c => new OrderService(c.GetRequiredService<IEmailSender>()));
        string result = services.GetRequiredService<OrderService>().Place("t@est");
        Debug.Assert(result.StartsWith("FAKE", StringComparison.Ordinal));
        Console.WriteLine($"  test double: {result}");
    }
}
