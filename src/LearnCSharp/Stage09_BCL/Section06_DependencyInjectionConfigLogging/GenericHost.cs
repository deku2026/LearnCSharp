// LearnCSharp example (filled)
// Doc      : CSharp-阶段9-BCL-第6部分-依赖注入与配置日志.md
// Stage    : Stage09_BCL
// Section  : Section06_DependencyInjectionConfigLogging
// Item     : GenericHost
// Topic id : stage09/section06/generic_host
//
// 步骤 5：Host.CreateApplicationBuilder — DI + 配置 + 日志 + IHostedService

using System.Diagnostics;
using LearnCSharp.Topics;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace LearnCSharp.Stage09.Section06;

internal static class GenericHost
{
    private sealed class WorkerService : IHostedService
    {
        private readonly ILogger<WorkerService> _logger;
        public List<string> Events { get; } = [];

        public WorkerService(ILogger<WorkerService> logger) => _logger = logger;

        public Task StartAsync(CancellationToken cancellationToken)
        {
            Events.Add("worker:start");
            _logger.LogInformation("Worker started");
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Events.Add("worker:stop");
            _logger.LogInformation("Worker stopped");
            return Task.CompletedTask;
        }
    }

    [LearnTopic("stage09/section06/generic_host")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== GenericHost ===");
        DemoRealHost().GetAwaiter().GetResult();
        return 0;
    }

    private static async Task DemoRealHost()
    {
        Console.WriteLine("-- Host.CreateApplicationBuilder + IHostedService lifecycle --");

        HostApplicationBuilder builder = Host.CreateApplicationBuilder([]);
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.SetMinimumLevel(LogLevel.Information);
        builder.Services.AddSingleton<WorkerService>();
        builder.Services.AddHostedService(sp => sp.GetRequiredService<WorkerService>());

        using IHost host = builder.Build();
        WorkerService worker = host.Services.GetRequiredService<WorkerService>();

        // Start/Stop without blocking forever (real apps use host.RunAsync until shutdown)
        await host.StartAsync();
        Debug.Assert(worker.Events.Contains("worker:start"));
        await host.StopAsync();
        Debug.Assert(worker.Events is ["worker:start", "worker:stop"]);
        Console.WriteLine($"  events: [{string.Join(", ", worker.Events)}]");
        Console.WriteLine("  production: await host.RunAsync() until SIGTERM/Ctrl+C");
    }
}
