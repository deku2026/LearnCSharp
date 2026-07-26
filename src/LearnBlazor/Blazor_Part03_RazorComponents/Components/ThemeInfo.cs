namespace LearnBlazor.Blazor_Part03_RazorComponents.Components;

public sealed record ThemeInfo(string Name, string Background, string Foreground)
{
    public static readonly ThemeInfo Default = new("Default", "#f0f0f3", "#1a1c1e");
    public static readonly ThemeInfo Dark = new("Dark", "#1a1c1e", "#e2e2e5");
}
