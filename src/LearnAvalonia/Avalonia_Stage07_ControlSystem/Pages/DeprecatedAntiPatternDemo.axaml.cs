// Doc:    第4部分-阶段7-控件体系.md §J1
// Title:  过时·实验·反模式
// Stage:  Stage07
// Accept: 点名已删除 API 与替代
// Notes:  只读场景上 DataGrid → TableView；Watermark → PlaceholderText；旧 FileDialog → StorageProvider。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

public partial class DeprecatedAntiPatternDemo : UserControl
{
    public DeprecatedAntiPatternDemo()
    {
        InitializeComponent();
    }
}
