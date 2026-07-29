using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage24_MauiAppBuilderDI;

public partial class MainPage : ContentPage
{
    public MainPage(LifetimeProbe lifetimeProbe, CompositionLedger ledger)
    {
        InitializeComponent();
        StageHost.Content = new LearningStageView(StageCatalog.Create(lifetimeProbe, ledger));
    }
}
