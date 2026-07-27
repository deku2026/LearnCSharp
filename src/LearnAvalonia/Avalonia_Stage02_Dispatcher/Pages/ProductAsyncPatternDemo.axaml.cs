// Doc:    第2部分-阶段2-Dispatcher线程与UI亲和性.md §9
// Title:  模式汇编：产品级异步 UI
// Stage:  Stage02
// Accept: 可取消加载 + 节流搜索
// Notes:  展示可取消加载与节流搜索两个产品级模式。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

public partial class ProductAsyncPatternDemo : UserControl
{
    public ProductAsyncPatternDemo()
    {
        InitializeComponent();
    }
}
