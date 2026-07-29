namespace LearnMaui.Maui_Stage18_PerformanceDiagnostics;

public partial class App : Application
{
    private readonly MainPage _mainPage;
    private readonly PerformanceTelemetry _telemetry;

    public App(MainPage mainPage, PerformanceTelemetry telemetry)
    {
        using PerformanceTelemetry.OperationScope operation =
            telemetry.StartOperation("app.initialize");
        InitializeComponent();
        _mainPage = mainPage;
        _telemetry = telemetry;
    }

    protected override Window CreateWindow(IActivationState? activationState)
    {
        using PerformanceTelemetry.OperationScope operation =
            _telemetry.StartOperation("app.create_window");
        return new Window(_mainPage);
    }
}
