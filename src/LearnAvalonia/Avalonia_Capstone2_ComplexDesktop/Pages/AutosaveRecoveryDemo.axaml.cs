// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §5
// Title:  Autosave 与崩溃恢复
// Stage:  Capstone2
// Accept: 杀进程演练可恢复
// Notes:  防抖写 %LocalAppData%/Product/Recovery/；正常保存清 recovery；启动扫描提示恢复。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class AutosaveRecoveryDemo : UserControl
{
    public AutosaveRecoveryDemo()
    {
        InitializeComponent();
    }

    private void SimulateEditButton_Click(object? sender, RoutedEventArgs e)
    {
        if (RecoveryLog is not null)
        {
            RecoveryLog.Text = "模拟编辑 → 防抖写 %LocalAppData%/Product/Recovery/{documentId}.recovery\nRecovery 文件头（附录 B）：\n  {\"version\":1, \"documentId\":\"...\", \"originalPath\":null, \"savedAt\":\"...\", \"payload\":{}}";
        }
    }

    private void KillButton_Click(object? sender, RoutedEventArgs e)
    {
        if (RecoveryLog is not null)
        {
            RecoveryLog.Text = "演练步骤：\n1. 新建内容不点保存\n2. 等 autosave（防抖 1-2s 后写 recovery）\n3. 任务管理器结束进程\n4. 重启 → 发现 recovery → 模态「恢复/丢弃」\n5. 校验完整性（哈希/版本头）\n\n证据：日志时间戳 + 截图";
        }
    }

    private void NormalSaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (RecoveryLog is not null)
        {
            RecoveryLog.Text = "正常保存 → 写主文件 → 删对应 recovery 文件\n下次启动无 recovery 提示。";
        }
    }
}
