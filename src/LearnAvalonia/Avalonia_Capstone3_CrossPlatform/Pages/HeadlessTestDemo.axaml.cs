// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §10.2
// Title:  Headless 测试
// Stage:  Capstone3
// Accept: Avalonia.Headless 跨平台测试
// Notes:  Avalonia.Headless + xUnit；不依赖真实窗口；可在 CI 三平台跑。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class HeadlessTestDemo : UserControl
{
    public HeadlessTestDemo()
    {
        InitializeComponent();
    }
}
