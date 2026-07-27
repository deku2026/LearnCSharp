// Doc:    第9部分-阶段21-应用架构与工程结构.md §9
// Title:  Autosave 与崩溃恢复
// Stage:  Stage21
// Accept: 杀进程后可恢复
// Notes:  防抖写旁路 *.autosave；启动扫描 recovery；异常路径尝试 Flush。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class AutosaveRecoveryDemo : UserControl
{
    public AutosaveRecoveryDemo()
    {
        InitializeComponent();
    }
}
