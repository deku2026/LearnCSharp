// Doc:    第3部分-阶段5-LogicalTree-VisualTree-NameScope.md §2
// Title:  Visual Tree：布局、渲染与命中之树
// Stage:  Stage05
// Accept: 打印同一控件两棵树
// Notes:  交互式：点击按钮打印逻辑/视觉祖先链对照。
using System.Text;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.VisualTree;

namespace LearnAvalonia.Avalonia_Stage05_LogicalVisualTree.Pages;

public partial class VisualTreeDemo : UserControl
{
    public VisualTreeDemo()
    {
        InitializeComponent();
    }

    private void TargetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TargetButton is null || TreeDumpText is null)
        {
            return;
        }

        StringBuilder sb = new();
        sb.AppendLine("=== 逻辑祖先链 ===");
        List<ILogical> logical = [.. TargetButton.GetSelfAndLogicalAncestors().Reverse()];
        foreach (ILogical node in logical)
        {
            sb.AppendLine(node.GetType().Name);
        }

        sb.AppendLine();
        sb.AppendLine("=== 视觉祖先链 ===");
        int visualCount = 0;
        IEnumerable<object> visual = TargetButton.GetSelfAndVisualAncestors().Reverse();
        foreach (object node in visual)
        {
            sb.AppendLine(node.GetType().Name);
            visualCount++;
        }

        sb.AppendLine();
        sb.AppendLine($"逻辑链节点数：{logical.Count}，视觉链节点数：{visualCount}");
        sb.AppendLine("视觉链通常更长（含模板零件/装饰层）");
        TreeDumpText.Text = sb.ToString();
    }
}
