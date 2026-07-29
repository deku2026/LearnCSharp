using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage10_DevicePermissions;

public partial class MainPage : ContentPage
{
    public MainPage(IDeviceCapabilities capabilities)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(capabilities));
    }
}
