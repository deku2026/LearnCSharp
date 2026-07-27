// Doc:    第7部分-阶段16-统一平台抽象.md §6
// Title:  PlatformSettings / Hotkey
// Stage:  Stage16
// Accept: HotkeyConfiguration 读取
// Notes:  PlatformSettings.HotkeyConfiguration 提供 Ctrl+C 等系统快捷键映射。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage16_PlatformAbstraction.Pages;

public partial class PlatformSettingsDemo : UserControl
{
    public PlatformSettingsDemo()
    {
        InitializeComponent();
    }
}
