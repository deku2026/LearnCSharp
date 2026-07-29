using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage14_AccessibilityI18n;

public partial class MainPage : ContentPage
{
    public MainPage(LocalizedText localizedText)
    {
        InitializeComponent();
        Content = new LearningStageView(StageCatalog.Create(localizedText));
    }
}
