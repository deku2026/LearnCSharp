// Doc:    第9部分-阶段21-应用架构与工程结构.md §10
// Title:  多窗口状态
// Stage:  Stage21
// Accept: 持久化窗口 bounds 与文档列表
// Notes:  会话文件 ISessionRestorer；移动/Browser 简化为导航栈 + 草稿。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class MultiWindowStateDemo : UserControl
{
    public MultiWindowStateDemo()
    {
        InitializeComponent();
    }
}
