// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §9
// Title:  Tray · NativeMenu · DnD
// Stage:  Capstone2
// Accept: TrayIcon+NativeMenu + 文件拖入
// Notes:  Tray 用 NativeMenu（勿挂普通 Menu）；关闭主窗可隐藏到托盘（设置项）。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class TrayMenuDndDemo : UserControl
{
    public TrayMenuDndDemo()
    {
        InitializeComponent();
    }

    private void TrayButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TrayLog is not null)
        {
            TrayLog.Text = "TrayIcon + NativeMenu（§9.1）：\n  <!-- TrayIcon + NativeMenu：Show / 新建 / 退出 -->\n  关闭主窗可隐藏到托盘（设置项）\n\n12 变化检查表：Tray 用 NativeMenu（勿挂普通 Menu）";
        }
    }

    private void NativeMenuButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TrayLog is not null)
        {
            TrayLog.Text = "Native 菜单（§9.2）：\n  文件：打开/保存/退出\n  编辑：撤销/重做\n  帮助：关于\n  macOS：应用菜单结构符合平台习惯（尽力）";
        }
    }

    private void DndButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TrayLog is not null)
        {
            TrayLog.Text = "DnD（§9.3）：\n  从系统拖入文件到窗口 → 打开\n  格式走 12 transfer 模型\n  非法文件提示\n  剪贴板若用则 async/transfer（12 变化检查表）";
        }
    }
}
