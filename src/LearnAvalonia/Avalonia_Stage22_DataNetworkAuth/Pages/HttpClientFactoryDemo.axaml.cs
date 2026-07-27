// Doc:    第9部分-阶段22-数据存储网络与认证.md §1
// Title:  HttpClient 与 IHttpClientFactory
// Stage:  Stage22
// Accept: 命名客户端 + 委托处理程序
// Notes:  展示注册 + DelegatingHandler 管道；不发起真网络调用。
using Avalonia.Controls;
using Avalonia.Interactivity;
using Microsoft.Extensions.DependencyInjection;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class HttpClientFactoryDemo : UserControl
{
    public HttpClientFactoryDemo()
    {
        InitializeComponent();
    }

    private void BuildFactoryButton_Click(object? sender, RoutedEventArgs e)
    {
        // 真实 IHttpClientFactory 注册：构造 ServiceCollection + AddHttpClient。
        ServiceCollection services = new();
        services.AddHttpClient("api", (sp, client) =>
        {
            client.BaseAddress = new System.Uri("https://api.example.local/");
            client.Timeout = System.TimeSpan.FromSeconds(100);
        });
        // .AddHttpMessageHandler&lt;AuthHeaderHandler&gt;() 等（需额外注册 handler）
        ServiceProvider provider = services.BuildServiceProvider();
        System.Net.Http.IHttpClientFactory factory = provider.GetRequiredService<System.Net.Http.IHttpClientFactory>();
        System.Net.Http.HttpClient client = factory.CreateClient("api");

        if (FactoryLog is not null)
        {
            FactoryLog.Text = $"IHttpClientFactory 已注册并解析：\n  BaseAddress = {client.BaseAddress}\n  Timeout = {client.Timeout}\n注意：此 demo 不发起真网络调用；仅展示工厂构造。";
        }
    }
}
