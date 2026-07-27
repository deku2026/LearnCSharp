// Doc:    第10部分-阶段27-打包部署更新与CI.md §1
// Title:  Windows
// Stage:  Stage27
// Accept: MSIX/MSI 命令可演示
// Notes:  MSIX 现代；MSI 传统；Self-contained + Trim；Authenticode 签名。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class WindowsPackDemo : UserControl
{
    public WindowsPackDemo()
    {
        InitializeComponent();
    }
}
