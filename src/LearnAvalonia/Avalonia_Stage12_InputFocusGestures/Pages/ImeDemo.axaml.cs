// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.7
// Title:  IME 与文本输入
// Stage:  Stage12
// Accept: TextInput 上屏 + 组合期 Enter
// Notes:  IME 组合→TextInput 上屏；组合期 Enter 不提交。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class ImeDemo : UserControl
{
    public ImeDemo()
    {
        InitializeComponent();
    }
}
