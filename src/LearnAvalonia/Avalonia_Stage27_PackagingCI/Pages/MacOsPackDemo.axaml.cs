// Doc:    第10部分-阶段27-打包部署更新与CI.md §2
// Title:  macOS
// Stage:  Stage27
// Accept: .app/.dmg/签名/公证
// Notes:  .app bundle 结构；codesign 签名；xcrun notarytool 公证；dmg 打包。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage27_PackagingCI.Pages;

public partial class MacOsPackDemo : UserControl
{
    public MacOsPackDemo()
    {
        InitializeComponent();
    }
}
