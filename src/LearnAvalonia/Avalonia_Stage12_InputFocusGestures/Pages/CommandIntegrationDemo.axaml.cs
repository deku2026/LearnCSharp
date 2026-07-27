// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.12
// Title:  与命令系统集成
// Stage:  Stage12
// Accept: HotKey+Command + 隧道转发
// Notes:  HotKey 绑 Command；Window 隧道避免子控件吞热键。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class CommandIntegrationDemo : UserControl
{
    public CommandIntegrationDemo()
    {
        InitializeComponent();
    }
}
