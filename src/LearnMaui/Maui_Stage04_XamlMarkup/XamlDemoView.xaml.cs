namespace LearnMaui.Maui_Stage04_XamlMarkup;

public partial class XamlDemoView : ContentView
{
    public XamlDemoView(ProfileViewModel profile)
    {
        InitializeComponent();
        BindingContext = profile;
    }
}
