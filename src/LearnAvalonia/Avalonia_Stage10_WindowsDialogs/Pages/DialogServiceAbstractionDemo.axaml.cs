// Doc:    第5部分-阶段10-桌面窗口对话框与系统集成.md §2.13
// Title:  对话框服务抽象
// Stage:  Stage10
// Accept: IDialogService 让 VM 不引用 Window
// Notes:  VM-first：IDialogService 接口注入；VM 不引用具体 Window。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.Pages;

public partial class DialogServiceAbstractionDemo : UserControl
{
    public DialogServiceAbstractionDemo()
    {
        InitializeComponent();
    }
}
