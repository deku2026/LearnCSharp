using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage18_PerformanceDiagnostics;

public partial class MainPage : ContentPage
{
    public MainPage(PerformanceTelemetry telemetry)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(telemetry));
    }
}
