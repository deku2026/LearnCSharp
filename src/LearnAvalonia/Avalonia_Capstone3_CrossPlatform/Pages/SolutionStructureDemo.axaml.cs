// Doc:    第8部分-Capstone3-全平台Avalonia应用.md §1
// Title:  解决方案结构
// Stage:  Capstone3
// Accept: 桌面三平台 TFM 与项目分层
// Notes:  桌面三平台（Win/macOS/Linux）共用 net10.0 TFM；移动/Browser 见 roadmap 不实现。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone3_CrossPlatform.Pages;

public partial class SolutionStructureDemo : UserControl
{
    public SolutionStructureDemo()
    {
        InitializeComponent();
    }
}
