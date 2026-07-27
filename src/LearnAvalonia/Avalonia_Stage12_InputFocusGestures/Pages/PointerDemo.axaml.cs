// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.2
// Title:  指针：mouse / touch / pen
// Stage:  Stage12
// Accept: Pointer.Capture 与 CaptureLost
// Notes:  Pointer.Type/Pressure/Capture；CaptureLost 必须清理。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class PointerDemo : UserControl
{
    public PointerDemo()
    {
        InitializeComponent();
    }
}
