// Doc:    第7部分-阶段16-统一平台抽象.md §1
// Title:  TopLevel 统一抽象
// Stage:  Stage16
// Accept: 解释 TopLevel 三模式
// Notes:  TopLevel 是窗口/控件树根；三模式：Desktop(Window)/SingleView/Activity；安全访问器。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class TopLevelAbstractionDemo : UserControl
{
    public TopLevelAbstractionDemo()
    {
        InitializeComponent();
    }
}
