// Doc:    第7部分-阶段16-统一平台抽象.md §2.3
// Title:  沙盒/书签/安全
// Stage:  Stage16
// Accept: Starts/Bookmark 安全模型
// Notes:  macOS 沙盒：返回 IStorageItem 后续访问需保留引用或 Bookmark。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class StorageSecurityDemo : UserControl
{
    public StorageSecurityDemo()
    {
        InitializeComponent();
    }
}
