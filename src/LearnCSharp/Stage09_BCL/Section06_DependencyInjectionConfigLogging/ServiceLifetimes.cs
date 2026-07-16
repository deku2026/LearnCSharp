// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : ServiceLifetimes
// Topic id : stage09/section06/service_lifetimes
//
// 步骤 2：Transient / Scoped / Singleton + ValidateScopes 俘获依赖

using System.Diagnostics;
using LearnCSharp.Topics;
using Microsoft.Extensions.DependencyInjection;

namespace LearnCSharp.Stage09.Section06;

internal static class ServiceLifetimes
{
    private interface IId
    {
        Guid Value { get; }
    }

    private sealed class IdService : IId
    {
        public Guid Value { get; } = Guid.NewGuid();
    }

    private sealed class CapturingSingleton(IId id)
    {
        public Guid Captured => id.Value;
    }

    [LearnTopic("stage09/section06/service_lifetimes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ServiceLifetimes ===");
        DemoThreeLifetimes();
        DemoCaptiveDependencyValidation();
        return 0;
    }

    private static void DemoThreeLifetimes()
    {
        Console.WriteLine("-- Transient vs Scoped vs Singleton --");

        var transient = new ServiceCollection();
        transient.AddTransient<IId, IdService>();
        using (ServiceProvider p = transient.BuildServiceProvider())
        {
            Guid t1 = p.GetRequiredService<IId>().Value;
            Guid t2 = p.GetRequiredService<IId>().Value;
            Debug.Assert(t1 != t2);
            Console.WriteLine($"  Transient: {t1:N} != {t2:N}");
        }

        var scoped = new ServiceCollection();
        scoped.AddScoped<IId, IdService>();
        using (ServiceProvider p = scoped.BuildServiceProvider())
        {
            using (IServiceScope scope1 = p.CreateScope())
            {
                Guid s1 = scope1.ServiceProvider.GetRequiredService<IId>().Value;
                Guid s2 = scope1.ServiceProvider.GetRequiredService<IId>().Value;
                Debug.Assert(s1 == s2);
                Console.WriteLine($"  Scoped same scope: {s1:N}");
            }

            using IServiceScope scope2 = p.CreateScope();
            Guid s3 = scope2.ServiceProvider.GetRequiredService<IId>().Value;
            Console.WriteLine($"  Scoped new scope: {s3:N}");
        }

        var single = new ServiceCollection();
        single.AddSingleton<IId, IdService>();
        using (ServiceProvider p = single.BuildServiceProvider())
        {
            Guid g1 = p.GetRequiredService<IId>().Value;
            Guid g2 = p.GetRequiredService<IId>().Value;
            Debug.Assert(g1 == g2);
            Console.WriteLine($"  Singleton: {g1:N}");
        }
    }

    private static void DemoCaptiveDependencyValidation()
    {
        Console.WriteLine("-- captive dependency: ValidateScopes catches Singleton→Scoped --");
        var services = new ServiceCollection();
        services.AddScoped<IId, IdService>();
        services.AddSingleton<CapturingSingleton>();

        bool threw = false;
        try
        {
            using ServiceProvider provider = services.BuildServiceProvider(new ServiceProviderOptions
            {
                ValidateScopes = true,
                ValidateOnBuild = true
            });
            // If build succeeded, resolve should still surface the captive dependency
            _ = provider.GetRequiredService<CapturingSingleton>();
        }
        catch (Exception ex) when (ex is InvalidOperationException or AggregateException)
        {
            threw = true;
            string msg = ex is AggregateException agg
                ? string.Join(" | ", agg.Flatten().InnerExceptions.Select(e => e.Message))
                : ex.Message;
            Console.WriteLine($"  ValidateScopes/ValidateOnBuild: {ex.GetType().Name}");
            Console.WriteLine($"  {msg}");
            Debug.Assert(msg.Contains("scoped", StringComparison.OrdinalIgnoreCase)
                         || msg.Contains("singleton", StringComparison.OrdinalIgnoreCase));
        }

        Debug.Assert(threw, "expected exception for Singleton capturing Scoped");
        Console.WriteLine("  fix: inject IServiceScopeFactory into singleton; CreateScope when needed");
    }
}
