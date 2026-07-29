using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage19_TrimmingAOT;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
