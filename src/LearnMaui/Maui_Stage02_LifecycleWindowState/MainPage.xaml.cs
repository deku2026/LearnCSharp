using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage02_LifecycleWindowState;

public partial class MainPage : ContentPage
{
    private readonly LifecycleJournal _journal;
    private readonly PageWorkController _pageWork;

    public MainPage(
        LifecycleJournal journal,
        PageWorkController pageWork,
        StateStore stateStore)
    {
        InitializeComponent();
        _journal = journal;
        _pageWork = pageWork;
        Content = new LearningStageView(StageCatalog.Create(journal, pageWork, stateStore));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        _journal.Record("Page.OnAppearing");
        _ = ObservePageWorkAsync();
    }

    protected override void OnDisappearing()
    {
        _journal.Record("Page.OnDisappearing");
        _pageWork.Cancel();
        base.OnDisappearing();
    }

    private async Task ObservePageWorkAsync()
    {
        try
        {
            await _pageWork.RestartAsync(TimeSpan.FromSeconds(3));
            _journal.Record("Page work completed");
        }
        catch (OperationCanceledException)
        {
            _journal.Record("Page work canceled");
        }
    }
}
