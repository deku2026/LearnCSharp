using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage16_GraphicsMedia;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create());
    }
}
