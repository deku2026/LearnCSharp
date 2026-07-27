// Doc:    第3部分-阶段4-属性系统.md §10
// Title:  特殊 12 案例：Window.WindowState 变 Direct
// Stage:  Stage04
// Accept: 知道 WindowState 变 Direct 的后果
// Notes:  展示 WindowState 从 Styled 变 Direct 后样式不能设。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage04_PropertySystem.Pages;

public partial class WindowStateDirectDemo : UserControl
{
    public WindowStateDirectDemo()
    {
        InitializeComponent();
    }
}
