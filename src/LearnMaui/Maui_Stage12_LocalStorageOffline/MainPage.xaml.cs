using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage12_LocalStorageOffline;

public partial class MainPage : ContentPage
{
    public MainPage(IOfflineNoteRepository repository)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(repository));
    }
}
