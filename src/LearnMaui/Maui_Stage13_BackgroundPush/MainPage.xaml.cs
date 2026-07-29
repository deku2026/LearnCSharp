using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage13_BackgroundPush;

public partial class MainPage : ContentPage
{
    public MainPage(
        IBackgroundScheduler scheduler,
        BackgroundSyncCoordinator coordinator,
        INotificationPlatformBridge notifications,
        PushRegistrationStore pushRegistration)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(
            scheduler,
            coordinator,
            notifications,
            pushRegistration));
    }
}
