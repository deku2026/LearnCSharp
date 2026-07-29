using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage09_HandlerNativeControls;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
