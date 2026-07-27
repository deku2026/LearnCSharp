// Doc:    第9部分-阶段21-应用架构与工程结构.md §8
// Title:  状态/Undo/Redo/文档模型
// Stage:  Stage21
// Accept: Undo 最小演示
// Notes:  命令式 vs 快照式；连续输入合并；多文档 IDocumentService。
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage21_AppArchitecture.Pages;

public partial class UndoDocumentDemo : UserControl
{
    private readonly System.Collections.Generic.List<string> _undoStack = new();
    private readonly System.Collections.Generic.List<string> _redoStack = new();
    private string _text = string.Empty;

    public UndoDocumentDemo()
    {
        InitializeComponent();
    }

    private void AppendButton_Click(object? sender, RoutedEventArgs e)
    {
        if (UndoInput is null)
        {
            return;
        }
        // 连续敲击合并为一条（这里每次 push 一条；生产可合并相邻同类编辑）
        _undoStack.Add(_text);
        _redoStack.Clear();
        _text = _text + UndoInput.Text;
        if (UndoInput is not null)
        {
            UndoInput.Text = string.Empty;
        }
        UpdateStatus();
    }

    private void UndoButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_undoStack.Count == 0)
        {
            return;
        }
        _redoStack.Add(_text);
        _text = _undoStack[^1];
        _undoStack.RemoveAt(_undoStack.Count - 1);
        UpdateStatus();
    }

    private void RedoButton_Click(object? sender, RoutedEventArgs e)
    {
        if (_redoStack.Count == 0)
        {
            return;
        }
        _undoStack.Add(_text);
        _text = _redoStack[^1];
        _redoStack.RemoveAt(_redoStack.Count - 1);
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (UndoStatus is not null)
        {
            UndoStatus.Text = $"当前文本：{_text}\nUndo 栈深度：{_undoStack.Count} · Redo 栈深度：{_redoStack.Count}";
        }
    }
}
