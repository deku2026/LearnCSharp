using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage06_BindingMVVM;

public partial class MainPage : ContentPage
{
    private readonly NotesViewModel _viewModel;

    public MainPage(NotesViewModel viewModel)
    {
        InitializeComponent();
        _viewModel = viewModel;
        Content = new LearningStageView(StageCatalog.Create(viewModel));
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (_viewModel.Items.Count == 0)
        {
            _viewModel.RefreshCommand.Execute(null);
        }
    }

    protected override void OnDisappearing()
    {
        _viewModel.RefreshCommand.Cancel();
        base.OnDisappearing();
    }
}
