// Doc:    第11部分-阶段30-平台后端选读.md §3
// Title:  WinUI Composition 渲染/输入路径
// Stage:  Stage30
// Accept: 解释渲染/输入路径
// Notes:  WinUI Composition 通过 ICompositor / Windows.UI.Composition 做桌面合成；输入走 InputMessageSite。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage30_PlatformBackend.Pages;

public partial class WinUICompositionDemo : UserControl
{
    public WinUICompositionDemo()
    {
        InitializeComponent();
    }
}
