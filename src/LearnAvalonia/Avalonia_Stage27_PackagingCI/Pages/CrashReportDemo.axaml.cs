// Doc:    第10部分-阶段27-打包部署更新与CI.md §10
// Title:  崩溃上报与可观测
// Stage:  Stage27
// Accept: Sentry/崩溃上报
// Notes:  Sentry.NET / AppCenter；AOT 兼容；SourceLink 符号化栈。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class CrashReportDemo : UserControl
{
    public CrashReportDemo()
    {
        InitializeComponent();
    }
}
