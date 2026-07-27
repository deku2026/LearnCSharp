// Doc:    第7部分-阶段16-统一平台抽象.md §2.5
// Title:  平台差异（Storage）
// Stage:  Stage16
// Accept: Storage 三平台差异
// Notes:  Win32 全盘 / macOS 沙盒+Bookmark / Linux XDG Portal。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class StoragePlatformDiffDemo : UserControl
{
    public StoragePlatformDiffDemo()
    {
        InitializeComponent();
    }
}
