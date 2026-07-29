using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage04_XamlMarkup;

public partial class MainPage : ContentPage
{
    public MainPage(ProfileViewModel profile)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(profile));
    }
}
