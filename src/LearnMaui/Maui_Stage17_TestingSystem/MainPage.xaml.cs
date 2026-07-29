using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage17_TestingSystem;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
