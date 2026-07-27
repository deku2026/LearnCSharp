// Doc:    第10部分-阶段25-性能与诊断.md §8
// Title:  Overdraw、Compositor、帧率
// Stage:  Stage25
// Accept: 帧率与 overdraw 说明
// Notes:  Compositor 批量合成；Overdraw 多层透明叠加；RenderTimer 监控帧率。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class OverdrawFpsDemo : UserControl
{
    public OverdrawFpsDemo()
    {
        InitializeComponent();
    }
}
