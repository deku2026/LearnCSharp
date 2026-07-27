// Doc:    第9部分-阶段21-应用架构与工程结构.md §13
// Title:  授权许可边界
// Stage:  Stage21
// Accept: 功能开关 + 校验服务接口
// Notes:  开源 MIT vs Pro 控件；许可检查勿阻塞 UI 线程；插件许可证与主程序分离。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class LicensingDemo : UserControl
{
    public LicensingDemo()
    {
        InitializeComponent();
    }
}
