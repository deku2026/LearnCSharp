// Doc:    第10部分-阶段24-测试体系.md §8
// Title:  平台差异
// Stage:  Stage24
// Accept: 三平台测试差异表
// Notes:  Windows/macOS/Linux；Linux 需 Xvfb（旧版）；字体差异影响 Visual regression。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage24_TestingSystem.Pages;

public partial class PlatformDiffDemo : UserControl
{
    public PlatformDiffDemo()
    {
        InitializeComponent();
    }
}
