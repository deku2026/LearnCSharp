// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §2.1
// Title:  共享什么
// Stage:  Capstone3
// Accept: Views/VM/Service contracts 共享
// Notes:  共享 Views/VMs/Service contracts；平台差异点暴露为接口。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class SharedLayerDemo : UserControl
{
    public SharedLayerDemo()
    {
        InitializeComponent();
    }
}
