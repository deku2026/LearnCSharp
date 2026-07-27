// Doc:    第9部分-阶段22-数据存储网络与认证.md §7
// Title:  安全存储 ISecureStore
// Stage:  Stage22
// Accept: Preferences 与 SecureStore 分离
// Notes:  平台映射；键名常量；禁止 refresh_token 写 Preferences 明文。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class SecureStoreDemo : UserControl
{
    public SecureStoreDemo()
    {
        InitializeComponent();
    }

    private void SetSecretButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SecureLog is not null)
        {
            SecureLog.Text = "ISecureStore.SetAsync(\"auth.refresh\", \"<模拟 refresh_token>\")\n→ Windows: Credential Manager / DPAPI\n→ macOS: Keychain\n→ Linux: Secret Service (libsecret) / 降级加密文件\n→ iOS/Android: Keystore-backed\n→ Browser: 无对等；HttpOnly Cookie / 后端会话";
        }
    }

    private void GetSecretButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SecureLog is not null)
        {
            SecureLog.Text = "ISecureStore.GetAsync(\"auth.refresh\") → 返回模拟 token（重启仍在）\n键名常量集中定义，避免魔法字符串散落：\n  public static class SecretKeys { public const string Refresh = \"auth.refresh\"; }";
        }
    }

    private void ClearButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SecureLog is not null)
        {
            SecureLog.Text = "ISecureStore.RemoveAsync(\"auth.refresh\") → 注销时清 token\n本地清 + 可选 RP 发起注销端点 + 清 WebView cookie（若用嵌入登录）";
        }
    }
}
