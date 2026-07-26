namespace LearnBlazor.Blazor_Part04_FrontendStateArchitecture.Services;

public sealed class ThemeState : IDisposable
{
    private string _theme = "light";
    public string Theme => _theme;
    public bool IsDark => _theme == "dark";

    public event Action? Changed;

    public void Toggle()
    {
        _theme = IsDark ? "light" : "dark";
        Changed?.Invoke();
    }

    public void Set(string theme)
    {
        if (theme is "light" or "dark")
        {
            _theme = theme;
            Changed?.Invoke();
        }
    }

    public void Dispose() => Changed = null;
}
