// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §13
// Title:  性能剖析
// Stage:  Capstone2
// Accept: 冷启动/加载/滚动/Tab 内存数字
// Notes:  dotnet-trace/dotnet-counters/DevTools/平台 Profiler；一切结论带数字。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class PerfProfilingDemo : UserControl
{
    public PerfProfilingDemo()
    {
        InitializeComponent();
    }
}
