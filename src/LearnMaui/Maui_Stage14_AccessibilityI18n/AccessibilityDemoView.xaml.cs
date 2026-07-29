namespace LearnMaui.Maui_Stage14_AccessibilityI18n;

public partial class AccessibilityDemoView : ContentView
{
    private readonly LocalizedText _localizedText;

    public AccessibilityDemoView(LocalizedText localizedText)
    {
        InitializeComponent();
        _localizedText = localizedText;
        _localizedText.CultureChanged += OnCultureChanged;
        ApplyCulture();
    }

    private void OnCultureChanged(object? sender, EventArgs e)
    {
        ApplyCulture();
    }

    private void ApplyCulture()
    {
        FlowDirection = _localizedText.GetFlowDirection();
        TitleLabel.Text = _localizedText["PageTitle"];
        SaveButton.Text = _localizedText["Save"];
        SemanticProperties.SetDescription(SaveButton, _localizedText["Save"]);
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        SemanticScreenReader.Default.Announce(_localizedText["SavedAnnouncement"]);
    }

    protected override void OnHandlerChanging(HandlerChangingEventArgs args)
    {
        if (args.NewHandler is null)
        {
            _localizedText.CultureChanged -= OnCultureChanged;
        }

        base.OnHandlerChanging(args);
    }
}
