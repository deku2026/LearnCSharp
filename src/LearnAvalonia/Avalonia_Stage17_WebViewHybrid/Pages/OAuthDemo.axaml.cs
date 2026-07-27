// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §6
// Title:  OAuth + WebAuthBroker
// Stage:  Stage17
// Accept: WebAuthenticationBroker.AuthenticateAsync
// Notes:  WebAuthenticationBroker.AuthenticateAsync 接收 requestUri + redirectUri，返回回调 URI。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class OAuthDemo : UserControl
{
    public OAuthDemo()
    {
        InitializeComponent();
    }
}
