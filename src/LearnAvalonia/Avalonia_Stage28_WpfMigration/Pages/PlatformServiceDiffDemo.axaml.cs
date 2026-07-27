// Doc:    第10部分-阶段28-WPF-WinUI-WinForms迁移与互操作.md §4
// Title:  平台服务差异（迁移时必铲的 Windows 假设）
// Stage:  Stage28
// Accept: Windows 假设铲除
// Notes:  注册表/WMI/WPF 专属 API；Avalonia 抽象层替代。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage28_WpfMigration.Pages;

public partial class PlatformServiceDiffDemo : UserControl
{
    public PlatformServiceDiffDemo()
    {
        InitializeComponent();
    }
}
