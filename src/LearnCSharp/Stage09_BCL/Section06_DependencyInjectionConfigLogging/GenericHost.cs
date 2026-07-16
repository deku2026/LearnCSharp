// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : GenericHost
// Topic id : stage09/section06/generic_host
//
// 步骤 5：Host 把 DI + 配置 + 日志串起来（BCL 教育版，无 Microsoft.Extensions.Hosting 包）

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage09.Section06;

internal static class GenericHost
{
    private interface IHostedService
    {
        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);
    }

    private sealed class WorkerService(string name) : IHostedService
    {
        public List<string> Events { get; } = [];

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Events.Add($"{name}:start");
            Console.WriteLine($"  [{name}] started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Events.Add($"{name}:stop");
            Console.WriteLine($"  [{name}] stopped");
            return Task.CompletedTask;
        }
    }

    private sealed class MiniHost
    {
        private readonly List<IHostedService> _services = [];
        private readonly Dictionary<Type, object> _di = [];

        public MiniHost ConfigureServices(Action<Dictionary<Type, object>> configure)
        {
            configure(_di);
            return this;
        }

        public MiniHost AddHostedService(IHostedService service)
        {
            _services.Add(service);
            return this;
        }

        public T GetRequiredService<T>() where T : notnull
            => (T)_di[typeof(T)];

        public async Task RunAsync(CancellationToken cancellationToken = default)
        {
            foreach (IHostedService s in _services)
                await s.StartAsync(cancellationToken);
            // real Host blocks until shutdown; we stop immediately for the demo
            foreach (IHostedService s in _services.AsEnumerable().Reverse())
                await s.StopAsync(cancellationToken);
        }
    }

    [LearnTopic("stage09/section06/generic_host")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericHost ===");
        DemoMiniHost().GetAwaiter().GetResult();
        DemoRealHostApiShape();
        return 0;
    }

    private static async Task DemoMiniHost()
    {
        Console.WriteLine("-- educational host: DI bag + hosted services lifecycle --");
        var worker = new WorkerService("worker");
        var host = new MiniHost()
            .ConfigureServices(services =>
            {
                services[typeof(string)] = "LearnCSharp";
                services[typeof(WorkerService)] = worker;
            })
            .AddHostedService(worker);

        string app = host.GetRequiredService<string>();
        Debug.Assert(app == "LearnCSharp");
        await host.RunAsync();
        Debug.Assert(worker.Events is ["worker:start", "worker:stop"]);
        Console.WriteLine($"  events: [{string.Join(", ", worker.Events)}]");
    }

    private static void DemoRealHostApiShape()
    {
        Console.WriteLine("-- production shape (Microsoft.Extensions.Hosting) --");
        Console.WriteLine("  var builder = Host.CreateApplicationBuilder(args);");
        Console.WriteLine("  builder.Services.AddSingleton<IFoo, Foo>();");
        Console.WriteLine("  builder.Services.AddHostedService<BackgroundWorker>();");
        Console.WriteLine("  builder.Configuration / builder.Logging already wired");
        Console.WriteLine("  await builder.Build().RunAsync();");
        Console.WriteLine("  ASP.NET Core WebApplication is a specialized host on the same model");
        Debug.Assert(true);
    }
}
