using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage23_Tizen;

public partial class MainPage : ContentPage
{
    public MainPage(IOptionalPlatformCapabilities capabilities)
    {
        InitializeComponent();
        StageHost.Content = new LearningStageView(StageCatalog.Create(capabilities));
    }
}
