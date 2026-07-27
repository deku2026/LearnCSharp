// Doc:    第4部分-Capstone1-生产级桌面基础应用.md §2
// Title:  架构建议与组合根
// Stage:  Capstone1
// Accept: DI 组合根清晰
// Notes:  逻辑分层：Desktop/App/ViewModels/Core/Infrastructure/Tests/Headless.Tests；禁止单文件上帝窗口。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone1_DesktopFoundation.Pages;

public partial class ArchitectureCompositionDemo : UserControl
{
    public ArchitectureCompositionDemo()
    {
        InitializeComponent();
    }
}
