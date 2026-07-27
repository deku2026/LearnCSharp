// Doc:    第9部分-阶段22-数据存储网络与认证.md §6
// Title:  Preferences
// Stage:  Stage22
// Accept: 主题进 Preferences
// Notes:  非敏感标记（主题/窗口态/功能开关缓存）；桌面 JSON 文件、移动平台偏好、Browser localStorage。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage22_DataNetworkAuth.Pages;

public partial class PreferencesDemo : UserControl
{
    public PreferencesDemo()
    {
        InitializeComponent();
    }

    private void SetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PrefInput is null)
        {
            return;
        }
        // 演示内存版 IPreferences（生产用桌面 JSON 文件）。
        string key = "theme";
        string value = PrefInput.Text ?? string.Empty;
        if (PrefLog is not null)
        {
            PrefLog.Text = $"SetAsync(\"{key}\", \"{value}\") 已写入 Preferences\n注意：此 demo 仅模拟；生产应持久化到 %LocalAppData%/MyApp/prefs.json";
        }
    }

    private void GetButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PrefLog is not null)
        {
            PrefLog.Text = "GetAsync(\"theme\") → \"Dark\"（模拟返回值）\n非敏感标记可进 Preferences；refresh_token 不可。";
        }
    }
}
