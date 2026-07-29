namespace LearnMaui.Maui_Stage06_BindingMVVM;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder.UseMauiApp<App>();
        builder.Services.AddSingleton<INotesService, InMemoryNotesService>();
        builder.Services.AddTransient<NotesViewModel>();
        builder.Services.AddTransient<MvvmDemoView>();
        builder.Services.AddTransient<MainPage>();
        return builder.Build();
    }
}
