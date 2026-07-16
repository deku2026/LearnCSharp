// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : ServiceLifetimes
// Topic id : stage09/section06/service_lifetimes
//
// 步骤 2：Transient / Scoped / Singleton + 俘获依赖陷阱（教育演示）

using System.Diagnostics;
using LearnCSharp.Topics;

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

    private enum Lifetime { Transient, Scoped, Singleton }

    private sealed class LifetimeContainer
    {
        private readonly Dictionary<Type, (Lifetime Life, Func<object> Factory)> _reg = [];
        private readonly Dictionary<Type, object> _singletons = [];
        private Dictionary<Type, object>? _scope;

        public void Add(Type type, Lifetime life, Func<object> factory)
            => _reg[type] = (life, factory);

        public IDisposable CreateScope()
        {
            _scope = [];
            return new Scope(this);
        }

        public T Resolve<T>() where T : notnull
        {
            if (!_reg.TryGetValue(typeof(T), out var entry))
                throw new InvalidOperationException($"missing {typeof(T).Name}");

            return entry.Life switch
            {
                Lifetime.Transient => (T)entry.Factory(),
                Lifetime.Singleton => (T)_singletons.GetValueOrDefault(typeof(T))
                                      ?? (T)(_singletons[typeof(T)] = entry.Factory()),
                Lifetime.Scoped => ResolveScoped<T>(entry.Factory),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        private T ResolveScoped<T>(Func<object> factory) where T : notnull
        {
            if (_scope is null)
                throw new InvalidOperationException("no active scope");
            if (_scope.TryGetValue(typeof(T), out object? existing))
                return (T)existing;
            object created = factory();
            _scope[typeof(T)] = created;
            return (T)created;
        }

        private sealed class Scope(LifetimeContainer owner) : IDisposable
        {
            public void Dispose() => owner._scope = null;
        }
    }

    [LearnTopic("stage09/section06/service_lifetimes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== ServiceLifetimes ===");
        DemoThreeLifetimes();
        DemoCaptiveDependency();
        return 0;
    }

    private static void DemoThreeLifetimes()
    {
        Console.WriteLine("-- Transient vs Scoped vs Singleton --");
        var c = new LifetimeContainer();
        c.Add(typeof(IId), Lifetime.Transient, () => new IdService());

        Guid t1 = c.Resolve<IId>().Value;
        Guid t2 = c.Resolve<IId>().Value;
        Debug.Assert(t1 != t2);
        Console.WriteLine($"  Transient: {t1:N} != {t2:N}");

        var scoped = new LifetimeContainer();
        scoped.Add(typeof(IId), Lifetime.Scoped, () => new IdService());
        using (scoped.CreateScope())
        {
            Guid s1 = scoped.Resolve<IId>().Value;
            Guid s2 = scoped.Resolve<IId>().Value;
            Debug.Assert(s1 == s2);
            Console.WriteLine($"  Scoped same scope: {s1:N}");
        }
        using (scoped.CreateScope())
        {
            Guid s3 = scoped.Resolve<IId>().Value;
            Console.WriteLine($"  Scoped new scope: {s3:N}");
        }

        var single = new LifetimeContainer();
        single.Add(typeof(IId), Lifetime.Singleton, () => new IdService());
        Guid g1 = single.Resolve<IId>().Value;
        Guid g2 = single.Resolve<IId>().Value;
        Debug.Assert(g1 == g2);
        Console.WriteLine($"  Singleton: {g1:N}");
    }

    private static void DemoCaptiveDependency()
    {
        Console.WriteLine("-- captive dependency: Singleton must not hold Scoped --");
        // Educational: if Singleton caches a Scoped Id, it never refreshes
        var root = new LifetimeContainer();
        root.Add(typeof(IId), Lifetime.Scoped, () => new IdService());
        IId? captured = null;
        using (root.CreateScope())
            captured = root.Resolve<IId>(); // pretend singleton took this reference
        // after scope ends, "captured" is still the old instance → stale (like captive DbContext)
        Debug.Assert(captured is not null);
        Console.WriteLine($"  captured scoped id survives scope end (bug): {captured.Value:N}");
        Console.WriteLine("  fix: inject IServiceScopeFactory into singleton; create scope when needed");
        Console.WriteLine("  MS.DI validates this in Development (ValidateScopes)");
    }
}
