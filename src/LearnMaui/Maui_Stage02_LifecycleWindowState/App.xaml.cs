namespace LearnMaui.Maui_Stage02_LifecycleWindowState;

public partial class App : Application
{
    private readonly MainPage _mainPage;
    private readonly LifecycleJournal _journal;

    public App(MainPage mainPage, LifecycleJournal journal)
    {
        InitializeComponent();
        _mainPage = mainPage;
        _journal = journal;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = new(_mainPage);
        _journal.Attach(window);
        return window;
    }
}
