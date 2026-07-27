// Doc:    第5部分-阶段12-输入焦点手势与IME.md §3
// Title:  跨平台差异
// Stage:  Stage12
// Accept: 右键/全局热键/IME/Meta 键差异
// Notes:  macOS ⌘ vs Win/Lin Ctrl/Super；IME 后端各异。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
