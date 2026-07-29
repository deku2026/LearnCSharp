using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage26_HandlerMapperLayout;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        StageHost.Content = new LearningStageView(StageCatalog.Create(LayoutLab));
    }

    private void OnSelectAllClicked(object? sender, EventArgs e)
    {
        NativeEntry.RequestSelectAll();
    }
}
