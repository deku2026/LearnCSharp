// Doc:    shared infrastructure
// Title:  TopicItem
// Stage:  Stage13
// Notes:  Immutable topic descriptor; DemoFactory lazily builds the demo UserControl.
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage13_GraphicsAnimation.ViewModels;

internal sealed record TopicItem(string Title, string Doc, string Accept, Func<Control> DemoFactory)
{
    public int Order { get; init; }
}
