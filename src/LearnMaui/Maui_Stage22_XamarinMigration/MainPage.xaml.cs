using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage22_XamarinMigration;

public partial class MainPage : ContentPage
{
    public MainPage(MigrationAuditor auditor)
    {
        InitializeComponent();
        StageHost.Content = new LearningStageView(StageCatalog.Create(auditor));
    }
}
