// Doc:    第6部分-阶段14-自定义控件.md §2.14
// Title:  AOT 友好
// Stage:  Stage14
// Accept: 避免反射；显式 axaml 编译
// Notes:  AOT 友好做法；PublishAot 样例实证。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class AotFriendlyDemo : UserControl
{
    public AotFriendlyDemo()
    {
        InitializeComponent();
    }
}
