// Doc:    第4部分-阶段6-布局系统.md §D7
// Title:  RelativePanel 与 UniformGrid
// Stage:  Stage06
// Accept: 使用 RelativePanel 约束与 UniformGrid 等分
// Notes:  RelativePanel 约束过多易难读；UniformGrid 单元格等大按顺序填充。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage06_LayoutSystem.Pages;

public partial class RelativeUniformPanelDemo : UserControl
{
    public RelativeUniformPanelDemo()
    {
        InitializeComponent();
    }
}
