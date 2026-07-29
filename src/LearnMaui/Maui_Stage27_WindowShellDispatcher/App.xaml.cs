namespace LearnMaui.Maui_Stage27_WindowShellDispatcher;

public partial class App : Application
{
    private readonly AppShell _shell;
    private readonly WindowJournal _windowJournal;
    private readonly DeepLinkCoordinator _deepLinks;

    public App(
        AppShell shell,
        WindowJournal windowJournal,
        DeepLinkCoordinator deepLinks)
    {
        InitializeComponent();
        _shell = shell;
        _windowJournal = windowJournal;
        _deepLinks = deepLinks;
        _deepLinks.AttachShell(shell);
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new(_shell);
        _windowJournal.Attach(window);
        window.Created += (_, _) => _deepLinks.MarkShellReady();
        return window;
    }

    protected override void OnAppLinkRequestReceived(Uri uri)
    {
        base.OnAppLinkRequestReceived(uri);
        _ = _deepLinks.NavigateAsync(uri, CancellationToken.None);
    }
}
