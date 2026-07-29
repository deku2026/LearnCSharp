using CommunityToolkit.Maui.Markup;

namespace LearnMaui.Maui_Stage04_XamlMarkup;

public sealed class MarkupDemoView : ContentView
{
    public MarkupDemoView(ProfileViewModel profile)
    {
        BindingContext = profile;
        Content = new VerticalStackLayout
        {
            Padding = 16,
            Spacing = 8,
            Children =
            {
                new Label { FontSize = 22 }
                    .Bind(
                        Label.TextProperty,
                        static (ProfileViewModel model) => model.DisplayName),
                new Label { Text = "The same ViewModel, expressed with C# Markup." },
                new CollectionView
                {
                    ItemsSource = profile.Skills,
                    SelectionMode = SelectionMode.None,
                    ItemTemplate = new DataTemplate(() =>
                    {
                        Label name = new();
                        name.SetBinding(Label.TextProperty, nameof(ProfileItem.Name));
                        return name;
                    }),
                },
            },
        };
    }
}
