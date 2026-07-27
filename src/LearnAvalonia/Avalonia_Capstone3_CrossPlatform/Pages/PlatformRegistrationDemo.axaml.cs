// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §6
// Title:  平台差异注册（DI）
// Stage:  Capstone3
// Accept: DI 容器选型 + 平台注册
// Notes:  Microsoft.Extensions.DependencyInjection；平台在 Host 装配时注册实现。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class PlatformRegistrationDemo : UserControl
{
    public PlatformRegistrationDemo()
    {
        InitializeComponent();
    }
}
