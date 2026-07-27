// Doc:    第9部分-阶段21-应用架构与工程结构.md §5
// Title:  DI 与 Hosting 组合根
// Stage:  Stage21
// Accept: BuildServiceProvider 仅在 host
// Notes:  真实 ServiceCollection/ServiceProvider 构造演示，不接 Avalonia 生命周期。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class DiHostingDemo : UserControl
{
    public DiHostingDemo()
    {
        InitializeComponent();
    }

    private void BuildContainerButton_Click(object? sender, RoutedEventArgs e)
    {
        // 真实 DI 容器构造：仅 host/测试入口可调 BuildServiceProvider。
        ServiceCollection services = new();
        services.AddSingleton<IGreetingService, GreetingService>();
        services.AddTransient<GreetingViewModel>();
        ServiceProvider provider = services.BuildServiceProvider();

        GreetingViewModel vm = provider.GetRequiredService<GreetingViewModel>();
        if (DiLog is not null)
        {
            DiLog.Text = $"已构造 ServiceProvider\n服务数：{services.Count}\nIGreetingService → {provider.GetRequiredService<IGreetingService>().GetType().Name}\nGreetingViewModel.SayHello() → {vm.SayHello()}";
        }
    }

    private void ScopeButton_Click(object? sender, RoutedEventArgs e)
    {
        ServiceCollection services = new();
        services.AddSingleton<IGreetingService, GreetingService>();
        services.AddTransient<GreetingViewModel>();
        ServiceProvider provider = services.BuildServiceProvider();

        System.Diagnostics.Stopwatch sw = System.Diagnostics.Stopwatch.StartNew();
        int transientCount = 0;
        for (int i = 0; i < 3; i++)
        {
            using IServiceScope scope = provider.CreateScope();
            GreetingViewModel vm = scope.ServiceProvider.GetRequiredService<GreetingViewModel>();
            transientCount++;
            // 读 vm.SayHello() 确保每次 scope 解析出独立 transient 实例（CodeQL: useless assignment）。
            string hello = vm.SayHello();
            if (DiLog is not null && i == 2)
            {
                DiLog.Text = $"3 个独立 scope，每个 scope 内 GreetingViewModel 为 transient：{transientCount} 次创建\n末次 SayHello()={hello}\nIGreetingService 全局 singleton：{provider.GetRequiredService<IGreetingService>().GetHash()}";
            }
        }
        sw.Stop();
    }

    // 极简服务/VM 示意：组合根之外不应直接 new。
    private interface IGreetingService
    {
        string Greet(string name);
        int GetHash();
    }

    private sealed class GreetingService : IGreetingService
    {
        private readonly int _id = System.Random.Shared.Next(1, 10000);
        public string Greet(string name) => $"Hello, {name}! (svc#{_id})";
        public int GetHash() => _id;
    }

    private sealed class GreetingViewModel
    {
        private readonly IGreetingService _svc;
        public GreetingViewModel(IGreetingService svc) => _svc = svc;
        public string SayHello() => _svc.Greet("Avalonia");
    }
}
