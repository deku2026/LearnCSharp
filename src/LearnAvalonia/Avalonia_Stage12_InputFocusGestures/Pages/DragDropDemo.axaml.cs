// Doc:    第5部分-阶段12-输入焦点手势与IME.md §2.8
// Title:  拖放 DnD 与 IAsyncDataTransfer
// Stage:  Stage12
// Accept: DoDragDropAsync + DataTransfer
// Notes:  12 唯一路径：IAsyncDataTransfer；回调里勿回 UI 线程。
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

public partial class DragDropDemo : UserControl
{
    public DragDropDemo()
    {
        InitializeComponent();
    }
}
