// Doc:    第10部分-Capstone4-自定义控件渲染与Native-AOT.md §1.2
// Title:  StyledProperty + DirectProperty
// Stage:  Capstone4
// Accept: 区分两种 property 注册
// Notes:  StyledProperty = 后备存储；DirectProperty = 字段+委托 getter/setter。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Capstone4_CustomControlAOT.Pages;

public partial class PropertyTypesDemo : UserControl
{
    public PropertyTypesDemo()
    {
        InitializeComponent();
    }
}
