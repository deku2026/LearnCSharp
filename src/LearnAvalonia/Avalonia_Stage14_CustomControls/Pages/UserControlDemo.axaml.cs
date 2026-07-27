// Doc:    第6部分-阶段14-自定义控件.md §2.1
// Title:  UserControl 快路径
// Stage:  Stage14
// Accept: App 内复用块；难换肤
// Notes:  UserControl 自带 axaml；适合 App 内复用块。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class UserControlDemo : UserControl
{
    public UserControlDemo()
    {
        InitializeComponent();
    }
}
