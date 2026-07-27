// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.11
// Title:  浏览器限制
// Stage:  Stage12
// Accept: 剪贴板/热键/捕获在浏览器门控
// Notes:  浏览器剪贴板/热键/捕获均门控；放用户手势调用栈。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class BrowserLimitsDemo : UserControl
{
    public BrowserLimitsDemo()
    {
        InitializeComponent();
    }
}
