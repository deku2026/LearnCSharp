namespace LearnMaui.Maui_Stage06_BindingMVVM;

public partial class MvvmDemoView : ContentView
{
    public MvvmDemoView(NotesViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
