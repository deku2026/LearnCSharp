// Doc:    第10部分-阶段24-测试体系.md §7
// Title:  CI 集成
// Stage:  Stage24
// Accept: dotnet test + Headless 跑通
// Notes:  dotnet test 自动启动 Headless；CI matrix 覆盖 windows/ubuntu/macos。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class CiIntegrationDemo : UserControl
{
    public CiIntegrationDemo()
    {
        InitializeComponent();
    }
}
