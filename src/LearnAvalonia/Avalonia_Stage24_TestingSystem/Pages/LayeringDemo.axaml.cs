// Doc:    第10部分-阶段24-测试体系.md §1
// Title:  分层职责与命名
// Stage:  Stage24
// Accept: Unit/Headless/Visual/Appium 职责
// Notes:  命名约定 *Tests.cs；测试项目与 app 项目分离；Headless 用 [AvaloniaFact]。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class LayeringDemo : UserControl
{
    public LayeringDemo()
    {
        InitializeComponent();
    }
}
