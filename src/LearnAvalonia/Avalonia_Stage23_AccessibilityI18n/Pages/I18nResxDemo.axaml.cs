// Doc:    第10部分-阶段23-无障碍国际化与文本.md §7
// Title:  国际化：RESX、Culture、复数、运行时切语言
// Stage:  Stage23
// Accept: 运行时切语言可演示
// Notes:  ResourceManager + .resx；CultureInfo.CurrentUICulture；复数 PluralRules；运行时切 UICulture。
using System.Globalization;
using Avalonia.Controls;
using Avalonia.Interactivity;

namespace LearnAvalonia.Avalonia_Stage23_AccessibilityI18n.Pages;

public partial class I18nResxDemo : UserControl
{
    public I18nResxDemo()
    {
        InitializeComponent();
    }

    private void SetChinese_Click(object? sender, RoutedEventArgs e)
    {
        CultureInfo.CurrentUICulture = new CultureInfo("zh-CN");
        CultureInfo.CurrentCulture = new CultureInfo("zh-CN");
        UpdateStatus();
    }

    private void SetEnglish_Click(object? sender, RoutedEventArgs e)
    {
        CultureInfo.CurrentUICulture = new CultureInfo("en-US");
        CultureInfo.CurrentCulture = new CultureInfo("en-US");
        UpdateStatus();
    }

    private void UpdateStatus()
    {
        if (CultureStatus is not null)
        {
            string ui = CultureInfo.CurrentUICulture.Name;
            string sample = string.Format(CultureInfo.CurrentCulture, "{0:C}", 1280.5);
            string date = DateTime.Now.ToString("D", CultureInfo.CurrentCulture);
            CultureStatus.Text = "CurrentUICulture=" + ui + "\n货币格式=" + sample + "\n今天=" + date;
        }
    }
}
