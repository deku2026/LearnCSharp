// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §4
// Title:  Undo/Redo
// Stage:  Capstone2
// Accept: 10 步编辑可逐步撤销
// Notes:  每文档独立栈或工作区栈；Ctrl+Z/Ctrl+Y；连续输入合并；保存不自动清空栈。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class UndoRedoDemo : UserControl
{
    private readonly System.Collections.Generic.List<string> _undo = new();
    private readonly System.Collections.Generic.List<string> _redo = new();
    private string _text = string.Empty;
    private int _steps;

    public UndoRedoDemo()
    {
        InitializeComponent();
    }

    private void AppendButton_Click(object? sender, RoutedEventArgs e)
    {
        if (UndoInput is null)
        {
            return;
        }
        _steps++;
        _undo.Add(_text);
        _redo.Clear();
        _text = _text + UndoInput.Text;
        UndoInput.Text = string.Empty;
        Update();
    }

    private void UndoButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_undo.Count == 0)
        {
            return;
        }
        _redo.Add(_text);
        _text = _undo[^1];
        _undo.RemoveAt(_undo.Count - 1);
        Update();
    }

    private void RedoButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_redo.Count == 0)
        {
            return;
        }
        _undo.Add(_text);
        _text = _redo[^1];
        _redo.RemoveAt(_redo.Count - 1);
        Update();
    }

    private void RunTenButton_Click(object? sender, RoutedEventArgs e)
    {
        // 验收：10 步编辑可逐步撤销。
        for (int i = 1; i <= 10; i++)
        {
            _undo.Add(_text);
            _redo.Clear();
            _text = _text + $"S{i}";
            _steps++;
        }
        Update();
        if (UndoStatus is not null)
        {
            UndoStatus.Text += "\n\n已自动执行 10 步编辑（S1..S10）。点 Undo 10 次可逐步撤销。";
        }
    }

    private void Update()
    {
        if (UndoStatus is not null)
        {
            UndoStatus.Text = $"步骤：{_steps} · 当前文本：{_text}\nUndo 栈深度：{_undo.Count} · Redo 栈深度：{_redo.Count}";
        }
    }
}
