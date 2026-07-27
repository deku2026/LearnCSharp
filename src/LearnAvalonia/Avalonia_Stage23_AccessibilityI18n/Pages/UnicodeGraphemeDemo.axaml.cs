// Doc:    第10部分-阶段23-无障碍国际化与文本.md §6
// Title:  Unicode、Grapheme、Shaping、字体回退、Emoji、IME
// Stage:  Stage23
// Accept: StringInfo 字素拆解可演示
// Notes:  StringInfo.GetTextElementEnumerator；字素簇 ≠ 码点；IME Composition/TextInputClient。
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class UnicodeGraphemeDemo : UserControl
{
    public UnicodeGraphemeDemo()
    {
        InitializeComponent();
    }

    private void AnalyzeButton_Click(object? sender, RoutedEventArgs e)
    {
        if (SampleInput is null || ResultText is null)
        {
            return;
        }

        string text = SampleInput.Text ?? string.Empty;
        TextElementEnumerator enumerator = StringInfo.GetTextElementEnumerator(text);
        int graphemeCount = 0;
        int codePointCount = text.Length;
        while (enumerator.MoveNext())
        {
            graphemeCount++;
        }

        ResultText.Text = "字符串=\"" + text + "\"\n码点数(UTF-16 char)=" + codePointCount + "\n字素簇数=" + graphemeCount + "\n示例：👩‍👩‍👧‍👧 = 11 char / 1 grapheme；🏳️‍🌈 = 7 char / 1 grapheme";
    }
}
