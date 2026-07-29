using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage05_ControlsLayout;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
