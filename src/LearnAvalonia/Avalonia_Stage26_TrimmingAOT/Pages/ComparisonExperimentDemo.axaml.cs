// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §9
// Title:  对比实验设计（验收硬通货）
// Stage:  Stage26
// Accept: trim/AOT 体积对比表
// Notes:  同一 app 三个配置：默认/Trim/AOT；测体积+启动+内存。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class ComparisonExperimentDemo : UserControl
{
    public ComparisonExperimentDemo()
    {
        InitializeComponent();
    }
}
