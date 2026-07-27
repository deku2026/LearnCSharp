// Doc:    第7部分-阶段17-WebView与Hybrid-UI.md §10
// Title:  WASM 硬边界
// Stage:  Stage17
// Accept: Browser 端无 WebView
// Notes:  Avalonia WebAssembly 运行时不能嵌 NativeWebView（浏览器无嵌套 webview）。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage17_WebViewHybrid.Pages;

public partial class WasmBoundaryDemo : UserControl
{
    public WasmBoundaryDemo()
    {
        InitializeComponent();
    }
}
