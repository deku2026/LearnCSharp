namespace LearnMaui.Maui_Stage08_StylesThemesAnimation;

public partial class InteractionDemoView : ContentView
{
    private bool _selected;

    public InteractionDemoView()
    {
        InitializeComponent();
    }

    private void OnCardTapped(object? sender, TappedEventArgs e)
    {
        _selected = !_selected;
        VisualStateManager.GoToState(Card, _selected ? "Selected" : "Normal");
    }

    private void OnPointerEntered(object? sender, PointerEventArgs e)
    {
        VisualStateManager.GoToState(Card, "PointerOver");
    }

    private void OnPointerExited(object? sender, PointerEventArgs e)
    {
        VisualStateManager.GoToState(Card, _selected ? "Selected" : "Normal");
    }
}
