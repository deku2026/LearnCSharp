// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §10
// Title:  全局激活 · 单实例 · CLI
// Stage:  Capstone2
// Accept: 再双击不出现双实例
// Notes:  mutex/pipe + 第二进程发 args + 主进程 Activate+Open；ICommandLineActivation。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class SingleInstanceCliDemo : UserControl
{
    public SingleInstanceCliDemo()
    {
        InitializeComponent();
    }

    private void SingleInstanceButton_Click(object? sender, RoutedEventArgs e)
    {
        if (CliLog is not null)
        {
            CliLog.Text = "单实例（附录 D 伪码）：\n  if (!mutex.WaitOne(0))\n  {\n      IpcClient.Send(args);\n      return;\n  }\n  IpcServer.Start(args => Dispatcher.UIThread.Post(() => Activate(args)));";
        }
    }

    private void CliButton_Click(object? sender, RoutedEventArgs e)
    {
        if (CliLog is not null)
        {
            CliLog.Text = "CLI 激活：\n  myapp.exe path\\to\\file   → 打开文件\n  myapp.exe --new          → 新建文档\n  myapp.exe --goto item/42 → 导航\n  ICommandLineActivation 解析 → 导航/打开";
        }
    }

    private void FileAssocButton_Click(object? sender, RoutedEventArgs e)
    {
        if (CliLog is not null)
        {
            CliLog.Text = "文件关联：\n  Windows 安装器写注册表/稀疏清单\n  macOS Info.plist CFBundleDocumentTypes\n  Linux .desktop MimeType\n  应用内 IActivationHandler 统一处理\n\n验收：已运行时再双击文件关联或 CLI，不出现双实例抢文件";
        }
    }
}
