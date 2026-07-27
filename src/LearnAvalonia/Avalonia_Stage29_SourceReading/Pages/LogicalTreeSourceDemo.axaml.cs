// Doc:    第11部分-阶段29-必读子系统源码精读.md §6
// Title:  Logical Tree 源码精读
// Stage:  Stage29
// Accept: 列出 ILogical 关系接口
// Notes:  Avalonia.LogicalTree 命名空间下 ILogical / ILogicalRoot / LogicalTreeNode（internal）；展示关系接口。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class LogicalTreeSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public LogicalTreeSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Entries.Add(new ReflectionEntry("ILogical", "Avalonia.LogicalTree.ILogical"));
        Entries.Add(new ReflectionEntry("ILogicalRoot", "Avalonia.LogicalTree.ILogicalRoot"));
        Entries.Add(new ReflectionEntry("LogicalTreeNode", "internal — 算法实现 Parent/Children"));
        Entries.Add(new ReflectionEntry("  ILogical.Parent", "ILogical? — 逻辑父"));
        Entries.Add(new ReflectionEntry("  ILogical.LogicalChildren", "ILogicalList — 逻辑子集合"));
        Entries.Add(new ReflectionEntry("  ILogical.IsAttachedToLogicalTree", "bool — 是否已附着"));
        Entries.Add(new ReflectionEntry("附着生命周期", "AttachedToLogicalTree / DetachedFromLogicalTree"));
        Entries.Add(new ReflectionEntry("扩展方法", "GetLogicalAncestors / GetLogicalDescendants / GetLogicalParent"));
        Entries.Add(new ReflectionEntry("关系来源", "ContentControl.Content / ItemsControl.Items / 树控件"));
    }
}
