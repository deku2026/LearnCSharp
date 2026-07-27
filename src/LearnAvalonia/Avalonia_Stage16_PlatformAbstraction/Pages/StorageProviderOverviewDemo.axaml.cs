// Doc:    第7部分-阶段16-统一平台抽象.md §2.1
// Title:  StorageProvider 职责
// Stage:  Stage16
// Accept: StorageProvider 三件套
// Notes:  StorageProvider 统一文件/文件夹选择 + StorageItem 抽象 + Save/Read 简化。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class StorageProviderOverviewDemo : UserControl
{
    public StorageProviderOverviewDemo()
    {
        InitializeComponent();
    }
}
