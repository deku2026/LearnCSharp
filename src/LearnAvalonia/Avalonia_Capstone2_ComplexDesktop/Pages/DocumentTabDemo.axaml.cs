// Doc:    第9部分-Capstone2-复杂桌面生产应用.md §3
// Title:  Document / Tab 模型
// Stage:  Capstone2
// Accept: 多 Tab 文档 + 脏文档确认
// Notes:  IDocumentWorkspace 打开/关闭/激活/保存；至少单主窗多 Tab；多窗加分。
using System.Collections.ObjectModel;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Capstone2_ComplexDesktop.Pages;

public partial class DocumentTabDemo : UserControl
{
    public ObservableCollection<DocVm> Tabs { get; } = new();
    private int _seq;

    public DocumentTabDemo()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void NewTabButton_Click(object? sender, RoutedEventArgs e)
    {
        _seq++;
        DocVm doc = new($"文档 {_seq}", $"这是文档 {_seq} 的初始内容。");
        Tabs.Add(doc);
    }

    private void CloseTabButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TabList?.SelectedItem is DocVm doc)
        {
            if (doc.IsDirty)
            {
                if (TabLog is not null)
                {
                    TabLog.Text = $"关闭「{doc.Title}」：IsDirty=true → 应弹确认（保存/丢弃/取消）\n本 demo 直接移除模拟「丢弃」。";
                }
            }
            else if (TabLog is not null)
            {
                TabLog.Text = $"关闭「{doc.Title}」：IsDirty=false → 直接关闭，无需确认。";
            }
            Tabs.Remove(doc);
        }
    }

    private void MarkDirtyButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TabList?.SelectedItem is DocVm doc)
        {
            doc.Content += "（编辑过）";
            doc.IsDirty = true;
            if (TabLog is not null)
            {
                TabLog.Text = $"「{doc.Title}」已标记脏（IsDirty=true）。";
            }
        }
    }

    private void SaveButton_Click(object? sender, RoutedEventArgs e)
    {
        if (TabList?.SelectedItem is DocVm doc)
        {
            doc.IsDirty = false;
            if (TabLog is not null)
            {
                TabLog.Text = $"「{doc.Title}」已保存 → IsDirty=false。";
            }
        }
    }

    public sealed class DocVm
    {
        public string Title { get; set; }
        public string Content { get; set; }
        public bool IsDirty { get; set; }
        public DocVm(string title, string content)
        {
            Title = title;
            Content = content;
        }
    }
}
