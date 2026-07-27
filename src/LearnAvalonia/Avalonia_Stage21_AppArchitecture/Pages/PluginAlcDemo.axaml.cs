// Doc:    第9部分-阶段21-应用架构与工程结构.md §4
// Title:  插件与 AssemblyLoadContext
// Stage:  Stage21
// Accept: ALC 隔离与卸载限制
// Notes:  插件仅经 Abstractions；动态插件与 AOT 敌对。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class PluginAlcDemo : UserControl
{
    public PluginAlcDemo()
    {
        InitializeComponent();
    }
}
