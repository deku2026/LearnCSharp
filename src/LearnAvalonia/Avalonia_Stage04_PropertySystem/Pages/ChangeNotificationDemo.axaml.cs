// Doc:    第3部分-阶段4-属性系统.md §7
// Title:  变更通知与响应式编程接点
// Stage:  Stage04
// Accept: 写出 class handler 或 OnPropertyChanged 处理
// Notes:  展示 OnPropertyChanged 与 AvaloniaProperty/INPC 分工。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage04_PropertySystem.Pages;

public partial class ChangeNotificationDemo : UserControl
{
    public ChangeNotificationDemo()
    {
        InitializeComponent();
    }
}
