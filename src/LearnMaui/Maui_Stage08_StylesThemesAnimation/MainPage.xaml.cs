using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage08_StylesThemesAnimation;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
