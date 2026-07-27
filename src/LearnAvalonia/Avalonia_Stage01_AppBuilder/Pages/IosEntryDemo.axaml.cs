// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §8
// Title:  iOS scene-based 注意点
// Stage:  Stage01
// Accept: 说明 AppDelegate.Window 可能 null
// Notes:  展示 scene 生命周期与 Window 所有权注意事项。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class IosEntryDemo : UserControl
{
    public IosEntryDemo()
    {
        InitializeComponent();
    }
}
