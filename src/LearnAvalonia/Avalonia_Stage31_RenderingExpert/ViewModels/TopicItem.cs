// Doc:    shared infrastructure
// Title:  TopicItem
// Stage:  Stage31
// Notes:  Immutable topic descriptor; DemoFactory lazily builds the demo UserControl.
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.ViewModels;

internal sealed record TopicItem(string Title, string Doc, string Accept, Func<Control> DemoFactory)
{
    public int Order { get; init; }
}
