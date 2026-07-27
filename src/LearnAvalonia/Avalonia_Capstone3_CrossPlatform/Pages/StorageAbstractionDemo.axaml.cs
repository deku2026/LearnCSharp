// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §5
// Title:  存储抽象
// Stage:  Capstone3
// Accept: IFileStorage 接口 + 三平台实现
// Notes:  Core 定义 IFileStorage；三平台用 StorageProvider + 平台路径实现。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class StorageAbstractionDemo : UserControl
{
    public StorageAbstractionDemo()
    {
        InitializeComponent();
    }
}
