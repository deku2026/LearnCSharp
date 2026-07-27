// Doc:    第4部分-阶段7-控件体系.md §C2
// Title:  ItemsControl·ListBox·TreeView·Carousel
// Stage:  Stage07
// Accept: 使用 ListBox 选中项与 ItemTemplate
// Notes:  ItemsControl 纯呈现无选择；ListBox 单/多选；TreeView 懒加载；Carousel 横滑。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

public partial class ListTreeCarouselDemo : UserControl
{
    public ObservableCollection<string> Items { get; } = ["苹果", "香蕉", "樱桃", "日期"];

    public ListTreeCarouselDemo()
    {
        InitializeComponent();
        DataContext = this;
    }
}
