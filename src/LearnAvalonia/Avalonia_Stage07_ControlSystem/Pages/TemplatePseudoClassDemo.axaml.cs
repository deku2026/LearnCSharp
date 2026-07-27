// Doc:    第4部分-阶段7-控件体系.md §F1
// Title:  控件与模板·伪类·AutomationId
// Stage:  Stage07
// Accept: 为关键控件补 AutomationId
// Notes:  多数控件外观来自 ControlTheme/ControlTemplate；改外观优先样式属性/Classes → 换 ControlTheme → 最后才派生。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage07_ControlSystem.Pages;

public partial class TemplatePseudoClassDemo : UserControl
{
    public TemplatePseudoClassDemo()
    {
        InitializeComponent();
    }
}
