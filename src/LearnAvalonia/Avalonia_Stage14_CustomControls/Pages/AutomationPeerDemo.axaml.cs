// Doc:    第6部分-阶段14-自定义控件.md §2.11
// Title:  AutomationPeer
// Stage:  Stage14
// Accept: OnCreateAutomationPeer 名称/类型
// Notes:  AutomationPeer 最低要求：名称/类型/键盘路径。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage14_CustomControls.Pages;

public partial class AutomationPeerDemo : UserControl
{
    public AutomationPeerDemo()
    {
        InitializeComponent();
    }
}
