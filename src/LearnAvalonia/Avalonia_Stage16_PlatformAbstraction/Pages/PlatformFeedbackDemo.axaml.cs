// Doc:    第7部分-阶段16-统一平台抽象.md §7
// Title:  PlatformFeedback（触觉）
// Stage:  Stage16
// Accept: PlatformFeedback 触觉反馈
// Notes:  移动为主；桌面一般无触觉；IFeedbackReceiver。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class PlatformFeedbackDemo : UserControl
{
    public PlatformFeedbackDemo()
    {
        InitializeComponent();
    }
}
