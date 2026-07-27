// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.5
// Title:  焦点：scopes、Tab、FocusManager
// Stage:  Stage12
// Accept: Focus/IsFocusScope/GotFocus 导航方式
// Notes:  FocusScope 子树 Tab 循环；GotFocusEventArgs.NavigationMethod。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class FocusDemo : UserControl
{
    public FocusDemo()
    {
        InitializeComponent();
    }
}
