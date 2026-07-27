// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §2.2
// Title:  Host 做什么
// Stage:  Capstone3
// Accept: 桌面平台入口与生命周期
// Notes:  桌面用 IClassicDesktopStyleApplicationLifetime；SingleView/Activity 见 roadmap 不实现。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class HostRoleDemo : UserControl
{
    public HostRoleDemo()
    {
        InitializeComponent();
    }
}
