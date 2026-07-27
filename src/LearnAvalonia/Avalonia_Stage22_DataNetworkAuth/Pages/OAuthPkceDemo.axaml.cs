// Doc:    第9部分-阶段22-数据存储网络与认证.md §9
// Title:  OAuth2 / OIDC / PKCE
// Stage:  Stage22
// Accept: PKCE 流文档 + 演示
// Notes:  概念演示，不做真实 auth；公有客户端无 client_secret。
using System.Security.Cryptography;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class OAuthPkceDemo : UserControl
{
    public OAuthPkceDemo()
    {
        InitializeComponent();
    }

    private void GeneratePkceButton_Click(object? sender, RoutedEventArgs e)
    {
        // 真实 PKCE 值生成（高熵 verifier + BASE64URL-SHA256 challenge），但不发起真 auth。
        byte[] verifierBytes = RandomNumberGenerator.GetBytes(32);
        string verifier = Base64UrlEncode(verifierBytes);
        byte[] challengeBytes = SHA256.HashData(verifierBytes);
        string challenge = Base64UrlEncode(challengeBytes);
        if (PkceLog is not null)
        {
            PkceLog.Text = $"PKCE 值已生成（不发起真 auth）：\n  code_verifier  = {verifier}\n  code_challenge = {challenge}\n  method         = S256\n\n下一步（概念）：\n  1. 授权请求带 challenge\n  2. 收到 redirect?code=\n  3. 用 code + verifier 换 token（TokenEndpoint）\n  4. ISecureStore 保存";
        }
    }

    private static string Base64UrlEncode(byte[] data)
    {
        System.Text.StringBuilder sb = new(System.Convert.ToBase64String(data));
        sb.Replace('+', '-').Replace('/', '_');
        while (sb.Length > 0 && sb[^1] == '=')
        {
            sb.Length--;
        }
        return sb.ToString();
    }

    private void RefreshButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PkceLog is not null)
        {
            PkceLog.Text = "刷新流（概念）：\n  access 过期 → 用 refresh_token 刷新 → 旋转 refresh（若服务器支持）更新存储 → 刷新失败 → 重新登录\n  并发：多个 API 401 时单飞刷新（Semaphore）";
        }
    }

    private void LogoutButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PkceLog is not null)
        {
            PkceLog.Text = "注销（概念）：\n  本地清 token（ISecureStore.RemoveAsync）\n  可选 RP 发起注销端点\n  清 WebView cookie（若用嵌入登录）";
        }
    }
}
