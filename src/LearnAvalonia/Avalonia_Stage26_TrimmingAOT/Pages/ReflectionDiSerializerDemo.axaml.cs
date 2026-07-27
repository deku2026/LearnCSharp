// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §4
// Title:  反射、DI、序列化、插件
// Stage:  Stage26
// Accept: 反射替代方案
// Notes:  DI 用 SourceGen；序列化 STJ SourceGen；插件用接口+AssemblyLoadContext（AOT 不支持）。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class ReflectionDiSerializerDemo : UserControl
{
    public ReflectionDiSerializerDemo()
    {
        InitializeComponent();
    }
}
