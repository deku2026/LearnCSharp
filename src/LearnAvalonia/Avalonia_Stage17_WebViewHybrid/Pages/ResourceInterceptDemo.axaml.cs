// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §5
// Title:  资源拦截与虚拟主机
// Stage:  Stage17
// Accept: WebResourceRequested 拦截
// Notes:  WebResourceRequested 事件可拦截请求返回自定义内容；类似虚拟主机映射。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class ResourceInterceptDemo : UserControl
{
    public ResourceInterceptDemo()
    {
        InitializeComponent();
    }
}
