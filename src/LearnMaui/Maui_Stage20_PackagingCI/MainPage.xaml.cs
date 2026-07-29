using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage20_PackagingCI;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
