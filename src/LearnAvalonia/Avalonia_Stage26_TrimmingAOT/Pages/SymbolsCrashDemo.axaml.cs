// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §8
// Title:  符号与崩溃诊断
// Stage:  Stage26
// Accept: Native AOT 崩溃 dump
// Notes:  StripSymbols=true 去符号；DBG 模式保留 PDB；崩溃 dump 用 dotnet-dump。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class SymbolsCrashDemo : UserControl
{
    public SymbolsCrashDemo()
    {
        InitializeComponent();
    }
}
