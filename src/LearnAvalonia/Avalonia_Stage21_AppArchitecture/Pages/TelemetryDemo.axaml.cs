// Doc:    第9部分-阶段21-应用架构与工程结构.md §11
// Title:  遥测边界
// Stage:  Stage21
// Accept: 接口存在且默认安全
// Notes:  默认关闭/征求同意；可剥离 PII；企业版可完全关闭。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class TelemetryDemo : UserControl
{
    public TelemetryDemo()
    {
        InitializeComponent();
    }
}
