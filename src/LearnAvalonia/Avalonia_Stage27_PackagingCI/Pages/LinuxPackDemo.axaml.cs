// Doc:    第10部分-阶段27-打包部署更新与CI.md §3
// Title:  Linux 桌面
// Stage:  Stage27
// Accept: AppImage/Flatpak/deb/rpm
// Notes:  AppImage 单文件；Flatpak 沙箱；deb/rpm 发行版包；.desktop 文件。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class LinuxPackDemo : UserControl
{
    public LinuxPackDemo()
    {
        InitializeComponent();
    }
}
