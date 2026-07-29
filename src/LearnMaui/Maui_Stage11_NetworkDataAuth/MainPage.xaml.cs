using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage11_NetworkDataAuth;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
