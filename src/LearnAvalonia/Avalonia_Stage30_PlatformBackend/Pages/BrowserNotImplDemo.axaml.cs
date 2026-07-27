// Doc:    第11部分-阶段30-平台后端选读.md §9
// Title:  Browser / WASM (非桌面 — 见 roadmap)
// Stage:  Stage30
// Accept: 见 roadmap，桌面策略不实现
// Notes:  WASM 走 Avalonia.Browser + CanvasKit；本桌面主线不 build/run。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

public partial class BrowserNotImplDemo : UserControl
{
    public BrowserNotImplDemo()
    {
        InitializeComponent();
    }
}
