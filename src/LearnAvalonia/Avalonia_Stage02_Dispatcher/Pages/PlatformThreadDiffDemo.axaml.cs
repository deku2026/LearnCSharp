// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §10
// Title:  跨平台差异表
// Stage:  Stage02
// Accept: 覆盖 7 类宿主线程模型
// Notes:  展示 Win/macOS/Linux/Android/iOS/Browser/Headless 线程模型对照。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class PlatformThreadDiffDemo : UserControl
{
    public PlatformThreadDiffDemo()
    {
        InitializeComponent();
    }
}
