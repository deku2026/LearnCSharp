// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.6
// Title:  ItemsRepeater
// Stage:  Stage15
// Accept: ItemsRepeater 布局虚拟化
// Notes:  独立包 Avalonia.Controls.ItemsRepeater（retired 状态，仅兼容到 12.0）；无内置选择/滚动条。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class ItemsRepeaterDemo : UserControl
{
    public ObservableCollection<string> Items { get; } = [];

    public ItemsRepeaterDemo()
    {
        for (int i = 0; i < 500; i++)
        {
            Items.Add("项目 #" + (i + 1));
        }

        InitializeComponent();
        DataContext = this;
    }
}
