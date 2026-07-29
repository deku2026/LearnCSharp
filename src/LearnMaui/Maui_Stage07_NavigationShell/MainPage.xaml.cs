using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage07_NavigationShell;

public partial class MainPage : ContentPage
{
    public MainPage(
        INavigationService navigation,
        NavigationGate gate,
        NavigationJournal journal)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(navigation, gate, journal));
    }
}
