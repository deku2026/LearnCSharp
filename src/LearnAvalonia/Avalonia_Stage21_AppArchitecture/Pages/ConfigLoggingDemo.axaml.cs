// Doc:    第9部分-阶段21-应用架构与工程结构.md §6
// Title:  配置与日志
// Stage:  Stage21
// Accept: IOptions + 结构化日志
// Notes:  配置分层 + 用户设置 vs 应用配置；日志不记密钥。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class ConfigLoggingDemo : UserControl
{
    public ConfigLoggingDemo()
    {
        InitializeComponent();
    }
}
