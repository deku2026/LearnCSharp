using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage15_WindowsMacAdaptive;

public partial class MainPage : ContentPage
{
    public MainPage(
        WindowWorkspaceRegistry workspaces,
        IDesktopPlatformService desktopPlatform)
    {
        InitializeComponent();
        MenuBarItems.Add(DesktopMenuFactory.Create());
        Content = new LearningStageView(
            StageCatalog.Create(workspaces, desktopPlatform));
    }
}
