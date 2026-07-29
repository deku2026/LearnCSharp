using Android.App;
using Android.Content;
using Android.Content.PM;

namespace LearnMaui.Maui_Stage27_WindowShellDispatcher;

[Activity(
    Theme = "@style/Maui.SplashTheme",
    MainLauncher = true,
    LaunchMode = LaunchMode.SingleTop,
    ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation |
        ConfigChanges.UiMode | ConfigChanges.ScreenLayout |
        ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
[IntentFilter(
    [Intent.ActionView],
    Categories = [Intent.CategoryDefault, Intent.CategoryBrowsable],
    DataScheme = "learnmaui",
    DataHost = "detail")]
public class MainActivity : MauiAppCompatActivity
{
}
