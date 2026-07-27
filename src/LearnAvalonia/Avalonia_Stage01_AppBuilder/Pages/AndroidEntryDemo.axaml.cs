// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §7
// Title:  Android 12 入口模型
// Stage:  Stage01
// Accept: 列出 Android 12 正确类型与已删除 API
// Notes:  展示 AvaloniaAndroidApplication<TApp> + 非泛型 Activity 与迁移表。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class AndroidEntryDemo : UserControl
{
    public AndroidEntryDemo()
    {
        InitializeComponent();
    }
}
