// Doc:    shared infrastructure
// Title:  TopicItem
// Stage:  Stage10
// Notes:  Immutable topic descriptor; DemoFactory lazily builds the demo UserControl.
using Avalonia.Controls;

namespace LearnAvalonia.Avalonia_Stage10_WindowsDialogs.ViewModels;

internal sealed record TopicItem(string Title, string Doc, string Accept, Func<Control> DemoFactory)
{
    public int Order { get; init; }
}
