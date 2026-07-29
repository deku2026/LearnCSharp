using LearnMaui.Shared;
using Microsoft.Extensions.Logging;

namespace LearnMaui.Maui_Stage01_ArchitectureStartup;

public partial class MainPage : ContentPage
{
    public MainPage(
        IStartupClock clock,
        AppSession session,
        IServiceProvider services,
        ILogger<MainPage> logger)
    {
        InitializeComponent();
        logger.LogInformation("MainPage constructed for session {SessionId}", session.Id);
        Content = new LearningStageView(StageCatalog.Create(clock, session, services));
    }
}
