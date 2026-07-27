// Doc:    第11部分-阶段29-必读子系统源码精读.md §5
// Title:  Property Store / 优先级 源码精读
// Stage:  Stage29
// Accept: 解释优先级与 effective 值
// Notes:  AvaloniaObject 内部维护 effective value table；列出 SetValue 走的优先级。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage29_SourceReading.Pages;

public partial class PropertyStoreSourceDemo : UserControl
{
    public ObservableCollection<ReflectionEntry> Entries { get; } = new();

    public PropertyStoreSourceDemo()
    {
        InitializeComponent();
        Build();
        DataContext = this;
    }

    private void Build()
    {
        Entries.Add(new ReflectionEntry("EffectiveValue", "AvaloniaObject 内部稀疏表 — 按优先级存储"));
        Entries.Add(new ReflectionEntry("优先级 1", "Animation (SetValue with Animated)"));
        Entries.Add(new ReflectionEntry("优先级 2", "LocalValue (SetValue default)"));
        Entries.Add(new ReflectionEntry("优先级 3", "StyleTrigger (Style Selector + Setter)"));
        Entries.Add(new ReflectionEntry("优先级 4", "TemplatedParent TemplateBinding"));
        Entries.Add(new ReflectionEntry("优先级 5", "Inherited (inherits=true 向下)"));
        Entries.Add(new ReflectionEntry("优先级 6", "Default (注册 defaultValue)"));
        Entries.Add(new ReflectionEntry("SetCurrentValue", "不破 LocalValue — 用于布局/动画临时覆写"));
        Entries.Add(new ReflectionEntry("ClearValue", "恢复到下一优先级"));
        Entries.Add(new ReflectionEntry("Observable?  是", "PropertyChanged / Changed.AddClassHandler"));
        Entries.Add(new ReflectionEntry("依赖触发", "AffectsMeasure / AffectsRender / AffectsArrange"));
    }
}
