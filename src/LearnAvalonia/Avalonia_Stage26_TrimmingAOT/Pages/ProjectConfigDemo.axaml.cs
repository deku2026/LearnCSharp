// Doc:    第10部分-阶段26-Trimming与Native-AOT.md §1
// Title:  项目配置基础
// Stage:  Stage26
// Accept: PublishTrimmed/PublishAot 配置
// Notes:  csproj PublishTrimmed=true/PublishAot=true/InvariantGlobalization；dotnet publish -r RID。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage26_TrimmingAOT.Pages;

public partial class ProjectConfigDemo : UserControl
{
    public ProjectConfigDemo()
    {
        InitializeComponent();
    }
}
