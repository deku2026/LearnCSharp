// Doc:    第9部分-阶段21-应用架构与工程结构.md §12
// Title:  桌面生产补充
// Stage:  Stage21
// Accept: 单实例/IPC/CLI/文件关联/更新/迁移
// Notes:  抽象 IUpdateService/ISettingsMigration/ISingleInstanceApp。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class DesktopProductionDemo : UserControl
{
    public DesktopProductionDemo()
    {
        InitializeComponent();
    }
}
