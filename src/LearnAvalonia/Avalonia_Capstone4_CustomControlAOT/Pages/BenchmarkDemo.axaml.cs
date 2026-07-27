// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §4
// Title:  Benchmark 概念
// Stage:  Capstone4
// Accept: BenchmarkDotNet stub
// Notes:  本 demo 仅展示 BenchmarkDotNet 概念；不在 demo 中真跑（需单独 console project）。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class BenchmarkDemo : UserControl
{
    public BenchmarkDemo()
    {
        InitializeComponent();
    }
}
