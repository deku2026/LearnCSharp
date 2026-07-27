// Doc:    第10部分-阶段25-性能与诊断.md §7
// Title:  虚拟化
// Stage:  Stage25
// Accept: ListBox 虚拟化默认
// Notes:  ListBox 默认虚拟化；ItemsRepeater 自管；ScrollViewer.Content 不虚拟化。
using System.Collections.ObjectModel;
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage25_Performance.Pages;

public partial class VirtualizationDemo : UserControl
{
    public ObservableCollection<string> BigItems { get; } = [];

    public VirtualizationDemo()
    {
        for (int i = 0; i < 5000; i++)
        {
            BigItems.Add("项目 #" + (i + 1));
        }

        InitializeComponent();
        DataContext = this;
    }
}
