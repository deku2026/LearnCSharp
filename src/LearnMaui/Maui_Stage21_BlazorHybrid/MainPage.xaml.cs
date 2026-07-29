using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage21_BlazorHybrid;

public partial class MainPage : ContentPage
{
    private readonly HybridBridge _bridge;

    public MainPage(HybridBridge bridge)
    {
        InitializeComponent();
        _bridge = bridge;
        StageHost.Content = new LearningStageView(StageCatalog.Create());

        ToolbarItems.Add(new ToolbarItem
        {
            Text = "Hybrid 实验",
            Command = new Command(async () =>
                await Navigation.PushAsync(new HybridLabPage(_bridge))),
        });
    }
}
