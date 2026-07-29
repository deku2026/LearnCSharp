using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage25_BindableBindingXaml;

public partial class MainPage : ContentPage
{
    public MainPage(BindingLabViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
        StageHost.Content = new LearningStageView(StageCatalog.Create(viewModel));
    }
}
