using System.Globalization;
using System.Resources;

namespace LearnMaui.Maui_Stage14_AccessibilityI18n;

public sealed class LocalizedText
{
    private static readonly ResourceManager Resources = new(
        "LearnMaui.Maui_Stage14_AccessibilityI18n.Resources.Strings.AppResources",
        typeof(LocalizedText).Assembly);

    public event EventHandler? CultureChanged;

    public string this[string key] =>
        Resources.GetString(key, CultureInfo.CurrentUICulture) ?? $"[{key}]";

    public string FormatItemCount(int count)
    {
        string key = count == 1 ? "ItemCountOne" : "ItemCountMany";
        return string.Format(CultureInfo.CurrentCulture, this[key], count);
    }

    public void SetCulture(string cultureName)
    {
        CultureInfo culture = CultureInfo.GetCultureInfo(cultureName);
        CultureInfo.CurrentCulture = culture;
        CultureInfo.CurrentUICulture = culture;
        CultureChanged?.Invoke(this, EventArgs.Empty);
    }

    public FlowDirection GetFlowDirection()
    {
        return CultureInfo.CurrentUICulture.TextInfo.IsRightToLeft
            ? FlowDirection.RightToLeft
            : FlowDirection.LeftToRight;
    }
}
