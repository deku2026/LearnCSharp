// Doc:    第2部分-阶段1-AppBuilder与应用启动.md §2
// Title:  AppBuilder：配置而不是启动
// Stage:  Stage01
// Accept: 解释 Configure/UsePlatformDetect/Skia+HarfBuzz
// Notes:  展示 BuildAvaloniaApp 流式配置与 Skia+HarfBuzz 配对铁律。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage01_AppBuilder.Pages;

public partial class AppBuilderDemo : UserControl
{
    public AppBuilderDemo()
    {
        InitializeComponent();
    }
}
