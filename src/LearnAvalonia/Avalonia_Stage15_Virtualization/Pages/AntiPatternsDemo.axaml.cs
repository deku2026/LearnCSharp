// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.10
// Title:  反模式：虚拟化失败
// Stage:  Stage15
// Accept: 识别 5 种反模式
// Notes:  Infinity 视口 / 不定高模板 / 非 virtualizing panel / 容器内重活 / 容器内同步 IO。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class AntiPatternsDemo : UserControl
{
    public AntiPatternsDemo()
    {
        InitializeComponent();
    }
}
