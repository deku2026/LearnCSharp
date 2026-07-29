using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage28_CollectionViewRuntime;

public partial class MainPage : ContentPage
{
    private readonly CollectionLabViewModel _viewModel;

    public MainPage(
        CollectionLabViewModel viewModel,
        EssentialsProbe essentials)
    {
        InitializeComponent();
        _viewModel = viewModel;
        BindingContext = viewModel;
        StageHost.Content = new LearningStageView(StageCatalog.Create(viewModel, essentials));
    }

    private void OnCollectionScrolled(object? sender, ItemsViewScrolledEventArgs e)
    {
        _viewModel.TrackVisibleRange(
            e.FirstVisibleItemIndex,
            e.CenterItemIndex,
            e.LastVisibleItemIndex,
            e.VerticalDelta);
    }
}
