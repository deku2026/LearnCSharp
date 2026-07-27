// Doc:    第11部分-阶段31-渲染专家档.md §2
// Title:  Scene / Composition
// Stage:  Stage31
// Accept: 画出 composition 角色
// Notes:  Compositor 持有 CompositionTarget / Visual；提交 CompositionUpdate。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

public partial class SceneCompositionDemo : UserControl
{
    public SceneCompositionDemo()
    {
        InitializeComponent();
    }
}
