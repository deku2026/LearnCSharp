namespace LearnMaui.Maui_Stage15_WindowsMacAdaptive;

public partial class App : Application
{
    private readonly MainPage _mainPage;
    private readonly WindowWorkspaceRegistry _workspaces;

    public App(MainPage mainPage, WindowWorkspaceRegistry workspaces)
    {
        InitializeComponent();
        _mainPage = mainPage;
        _workspaces = workspaces;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new(_mainPage)
        {
            Title = "Adaptive MAUI Workspace",
        };
        _workspaces.Attach(window);
        return window;
    }
}
