// Doc:    第9部分-阶段22-数据存储网络与认证.md §10
// Title:  WebView 登录与系统浏览器
// Stage:  Stage22
// Accept: 三种登录方式对比
// Notes:  优先 Broker 或系统浏览器；嵌入 WebView 登录需安全评审。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class WebViewLoginDemo : UserControl
{
    public WebViewLoginDemo()
    {
        InitializeComponent();
    }
}
