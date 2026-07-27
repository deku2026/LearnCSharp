// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §8
// Title:  插件边界
// Stage:  Capstone2
// Accept: 无插件可跑 + 有插件菜单出现
// Notes:  IProductPlugin + IPluginHost；ALC 或简单 LoadFrom；失败日志+跳过。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class PluginDemo : UserControl
{
    public PluginDemo()
    {
        InitializeComponent();
    }

    private void LoadPluginButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PluginLog is not null)
        {
            PluginLog.Text = "加载示例插件（概念）：\n  目录 plugins/*/plugin.json + DLL（附录 C）\n  {\n    \"id\": \"sample-timestamp\",\n    \"name\": \"Sample Timestamp\",\n    \"entryAssembly\": \"SamplePlugin.dll\",\n    \"apiVersion\": \"1.0\"\n  }\n  → ALC 或简单 Assembly.Load（开发可简单，文档写清生产风险）\n  → IProductPlugin.Initialize(IPluginHost)";
        }
    }

    private void BadPluginButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PluginLog is not null)
        {
            PluginLog.Text = "坏插件处理：\n  失败：日志 + 跳过，主程序仍启动\n  不拖垮宿主\n  验收：无插件可跑 + 有插件菜单出现 + 坏插件不拖垮宿主";
        }
    }

    private void SamplePluginButton_Click(object? sender, RoutedEventArgs e)
    {
        if (PluginLog is not null)
        {
            PluginLog.Text = "示例插件贡献：\n  IPluginHost.RegisterMenu(new MenuRegistration(\"插入/时间戳\", ...))\n  → 菜单出现「插入时间戳」或「额外导出格式」";
        }
    }
}
