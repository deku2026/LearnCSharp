using System.Collections.ObjectModel;
using Microsoft.Maui.Controls.Shapes;

namespace LearnMaui.Shared;

public sealed class LearningStageView : ContentView, IDisposable
{
    private readonly LearningStage _stage;
    private readonly ObservableCollection<LearningTopic> _visibleTopics = [];
    private readonly SearchBar _searchBar;
    private readonly Picker _levelPicker;
    private readonly Label _resultLabel;
    private readonly ActivityIndicator _activityIndicator;
    private CancellationTokenSource? _labCancellation;
    private bool _disposed;

    public LearningStageView(LearningStage stage)
    {
        _stage = stage;

        _searchBar = new SearchBar
        {
            Placeholder = "搜索知识点、解释或代码证据",
        };
        SemanticProperties.SetHint(_searchBar, "筛选当前阶段的知识点");
        _searchBar.TextChanged += OnFilterChanged;

        _levelPicker = new Picker
        {
            Title = "难度",
            ItemsSource = new[] { "全部", "入门", "进阶", "高级", "专家" },
            SelectedIndex = 0,
        };
        _levelPicker.SelectedIndexChanged += OnFilterChanged;

        CollectionView topicList = new()
        {
            ItemsSource = _visibleTopics,
            EmptyView = new Label
            {
                Text = "没有匹配的知识点",
                HorizontalOptions = LayoutOptions.Center,
                Margin = new Thickness(16),
            },
            ItemTemplate = new DataTemplate(CreateTopicCard),
            SelectionMode = SelectionMode.None,
        };

        VerticalStackLayout labs = new() { Spacing = 10 };
        foreach (LearningLab lab in stage.Labs)
        {
            Button button = new()
            {
                Text = lab.Title,
                HorizontalOptions = LayoutOptions.Fill,
            };
            SemanticProperties.SetDescription(button, lab.Goal);
            button.Clicked += async (_, _) => await RunLabAsync(lab);
            labs.Children.Add(button);
        }

        _activityIndicator = new ActivityIndicator
        {
            IsVisible = false,
            HorizontalOptions = LayoutOptions.Start,
        };
        _resultLabel = new Label
        {
            Text = "选择一个实验，运行结果会显示在这里。",
            LineBreakMode = LineBreakMode.WordWrap,
        };

        Grid pageGrid = new()
        {
            RowDefinitions =
            {
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Auto),
                new RowDefinition(GridLength.Star),
            },
        };
        pageGrid.Add(CreateHeader(stage), 0, 0);
        pageGrid.Add(CreateFilterBar(), 0, 1);
        pageGrid.Add(new Grid
        {
            ColumnDefinitions =
            {
                new ColumnDefinition(new GridLength(3, GridUnitType.Star)),
                new ColumnDefinition(new GridLength(2, GridUnitType.Star)),
            },
            Children =
            {
                topicList,
                CreateLabPanel(labs),
            },
        }, 0, 2);

        Content = pageGrid;
        SizeChanged += OnSizeChanged;
        ApplyFilter();
    }

    private static View CreateHeader(LearningStage stage)
    {
        Grid header = new()
        {
            Padding = new Thickness(20, 18, 20, 14),
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Auto),
                new ColumnDefinition(GridLength.Star),
            },
            BackgroundColor = Color.FromArgb("#172554"),
        };
        Border badge = new()
        {
            BackgroundColor = Color.FromArgb("#38BDF8"),
            StrokeThickness = 0,
            Padding = new Thickness(12, 7),
            Content = new Label
            {
                Text = stage.StageLabel,
                FontAttributes = FontAttributes.Bold,
                TextColor = Color.FromArgb("#082F49"),
            },
        };
        Border title = new()
        {
            StrokeThickness = 0,
            Padding = new Thickness(14, 0, 0, 0),
            Content = new VerticalStackLayout
            {
                Spacing = 2,
                Children =
                {
                    new Label
                    {
                        Text = stage.Title,
                        FontSize = 22,
                        FontAttributes = FontAttributes.Bold,
                        TextColor = Colors.White,
                    },
                    new Label
                    {
                        Text = $"参考：{stage.Reference}",
                        FontSize = 12,
                        TextColor = Color.FromArgb("#BAE6FD"),
                    },
                },
            },
        };
        header.Add(badge, 0, 0);
        header.Add(title, 1, 0);
        return header;
    }

    private View CreateFilterBar()
    {
        Grid filters = new()
        {
            Padding = new Thickness(14, 8),
            ColumnDefinitions =
            {
                new ColumnDefinition(GridLength.Star),
                new ColumnDefinition(new GridLength(128)),
            },
            ColumnSpacing = 8,
        };
        filters.Add(_searchBar, 0, 0);
        filters.Add(_levelPicker, 1, 0);
        return filters;
    }

    private View CreateLabPanel(VerticalStackLayout labs)
    {
        ScrollView scroll = new()
        {
            Content = new VerticalStackLayout
            {
                Padding = new Thickness(16),
                Spacing = 12,
                Children =
                {
                    new Label
                    {
                        Text = "可运行实验",
                        FontSize = 20,
                        FontAttributes = FontAttributes.Bold,
                    },
                    new Label
                    {
                        Text = "实验直接调用本阶段代码；重复运行可观察生命周期、状态或平台差异。",
                        FontSize = 13,
                    },
                    labs,
                    _activityIndicator,
                    new Border
                    {
                        Padding = new Thickness(12),
                        Stroke = Color.FromArgb("#94A3B8"),
                        StrokeShape = new RoundRectangle { CornerRadius = 10 },
                        Content = _resultLabel,
                    },
                },
            },
        };
        Grid.SetColumn(scroll, 1);
        return scroll;
    }

    private static View CreateTopicCard()
    {
        Label level = new()
        {
            FontSize = 12,
            FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#0369A1"),
        };
        level.SetBinding(Label.TextProperty, nameof(LearningTopic.Level), stringFormat: "{0}");

        Label title = new()
        {
            FontSize = 18,
            FontAttributes = FontAttributes.Bold,
        };
        title.SetBinding(Label.TextProperty, nameof(LearningTopic.Title));

        Label explanation = new()
        {
            FontSize = 14,
            LineBreakMode = LineBreakMode.WordWrap,
        };
        explanation.SetBinding(Label.TextProperty, nameof(LearningTopic.Explanation));

        Label evidence = new()
        {
            FontFamily = "monospace",
            FontSize = 12,
            TextColor = Color.FromArgb("#475569"),
            LineBreakMode = LineBreakMode.WordWrap,
        };
        evidence.SetBinding(Label.TextProperty, nameof(LearningTopic.Evidence), stringFormat: "代码证据：{0}");

        return new Border
        {
            Margin = new Thickness(12, 6),
            Padding = new Thickness(14),
            Stroke = Color.FromArgb("#CBD5E1"),
            StrokeShape = new RoundRectangle { CornerRadius = 12 },
            Content = new VerticalStackLayout
            {
                Spacing = 6,
                Children = { level, title, explanation, evidence },
            },
        };
    }

    private async Task RunLabAsync(LearningLab lab)
    {
        await CancelRunningLabAsync();
        _labCancellation = new CancellationTokenSource();
        _activityIndicator.IsVisible = true;
        _activityIndicator.IsRunning = true;
        _resultLabel.Text = $"{lab.Goal}\n\n正在运行…";

        try
        {
            string result = await lab.RunAsync(_labCancellation.Token);
            _resultLabel.Text = $"{lab.Goal}\n\n{result}";
        }
        catch (OperationCanceledException)
        {
            _resultLabel.Text = $"{lab.Goal}\n\n实验已取消。";
        }
        catch (Exception exception)
        {
            _resultLabel.Text = $"{lab.Goal}\n\n{exception.GetType().Name}: {exception.Message}";
        }
        finally
        {
            _activityIndicator.IsRunning = false;
            _activityIndicator.IsVisible = false;
        }
    }

    private void OnFilterChanged(object? sender, EventArgs e)
    {
        ApplyFilter();
    }

    private void ApplyFilter()
    {
        string query = _searchBar.Text?.Trim() ?? string.Empty;
        LearningLevel? selectedLevel = _levelPicker.SelectedIndex switch
        {
            1 => LearningLevel.Beginner,
            2 => LearningLevel.Intermediate,
            3 => LearningLevel.Advanced,
            4 => LearningLevel.Expert,
            _ => null,
        };

        IEnumerable<LearningTopic> matches = _stage.Topics.Where(topic =>
            (selectedLevel is null || topic.Level == selectedLevel) &&
            (query.Length == 0 ||
             topic.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             topic.Explanation.Contains(query, StringComparison.OrdinalIgnoreCase) ||
             topic.Evidence.Contains(query, StringComparison.OrdinalIgnoreCase)));

        _visibleTopics.Clear();
        foreach (LearningTopic topic in matches)
        {
            _visibleTopics.Add(topic);
        }
    }

    private void OnSizeChanged(object? sender, EventArgs e)
    {
        if (Content is not Grid pageGrid || pageGrid.Children[2] is not Grid contentGrid)
        {
            return;
        }

        bool compact = Width < 760;
        contentGrid.ColumnDefinitions.Clear();
        contentGrid.RowDefinitions.Clear();
        if (compact)
        {
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(GridLength.Star));
            contentGrid.RowDefinitions.Add(new RowDefinition(new GridLength(3, GridUnitType.Star)));
            contentGrid.RowDefinitions.Add(new RowDefinition(new GridLength(2, GridUnitType.Star)));
            contentGrid.SetColumn(contentGrid.Children[0], 0);
            contentGrid.SetRow(contentGrid.Children[0], 0);
            contentGrid.SetColumn(contentGrid.Children[1], 0);
            contentGrid.SetRow(contentGrid.Children[1], 1);
        }
        else
        {
            contentGrid.RowDefinitions.Add(new RowDefinition(GridLength.Star));
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(3, GridUnitType.Star)));
            contentGrid.ColumnDefinitions.Add(new ColumnDefinition(new GridLength(2, GridUnitType.Star)));
            contentGrid.SetColumn(contentGrid.Children[0], 0);
            contentGrid.SetRow(contentGrid.Children[0], 0);
            contentGrid.SetColumn(contentGrid.Children[1], 1);
            contentGrid.SetRow(contentGrid.Children[1], 0);
        }
    }

    private async Task CancelRunningLabAsync()
    {
        if (_labCancellation is null)
        {
            return;
        }

        await _labCancellation.CancelAsync();
        _labCancellation.Dispose();
        _labCancellation = null;
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;
        _searchBar.TextChanged -= OnFilterChanged;
        _levelPicker.SelectedIndexChanged -= OnFilterChanged;
        _labCancellation?.Cancel();
        _labCancellation?.Dispose();
    }
}
