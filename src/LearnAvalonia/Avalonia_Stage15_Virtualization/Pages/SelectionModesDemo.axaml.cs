// Doc:    第6部分-阶段15-虚拟化与大数据UI.md §2.7
// Title:  选择模式
// Stage:  Stage15
// Accept: 区分 Single/Multiple/Extended
// Notes:  SelectionMode 枚举；虚拟化下选择行为不变。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage15_Virtualization.Pages;

public partial class SelectionModesDemo : UserControl
{
    public ObservableCollection<string> Items { get; } = [];

    public SelectionModesDemo()
    {
        for (int i = 0; i < 200; i++)
        {
            Items.Add("行 #" + (i + 1));
        }

        InitializeComponent();
        DataContext = this;
    }
}
