using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage05_ControlsLayout;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "页面容器与 Border",
                "ContentPage 提供页面壳，ScrollView 只承载一个内容树。Border 是现代卡片/描边/圆角容器，替代已过时的 Frame 主线。",
                "ControlsFactory creates Border and ScrollView"),
            new(LearningLevel.Beginner, "文本、按钮与图片",
                "Label 展示格式化文本，Button 触发 Command，Image 通过 MAUI 资源管道加载并选择合适 Aspect。图像尺寸应匹配显示需求。",
                "Label/Button/Image control matrix"),
            new(LearningLevel.Beginner, "文本输入",
                "Entry 适合单行，Editor 适合多行，SearchBar 表达搜索意图。键盘类型、ReturnType、最大长度、验证和可访问标签应一起设计。",
                "Entry, Editor and SearchBar"),
            new(LearningLevel.Beginner, "选择与范围控件",
                "Picker/DatePicker/TimePicker 选离散值，CheckBox/RadioButton/Switch 表布尔或单选，Slider/Stepper 调整数值；绑定到明确类型状态。",
                "CreateControlMatrix selection controls"),
            new(LearningLevel.Beginner, "进度反馈",
                "ActivityIndicator 表示不确定时长，ProgressBar 表示 0–1 可量化进度。加载状态应局部呈现而非依赖已过时的 Page.IsBusy。",
                "ActivityIndicator.IsRunning and ProgressBar.Progress"),
            new(LearningLevel.Intermediate, "CollectionView 主列表",
                "大量数据使用虚拟化 CollectionView，并给 DataTemplate 正确 x:DataType；不要用 StackLayout+BindableLayout 承载无限列表。",
                "CollectionView in ControlsFactory"),
            new(LearningLevel.Intermediate, "Carousel、Swipe 与 Refresh",
                "CarouselView 与 IndicatorView 表分页内容；SwipeView 暴露快捷操作；RefreshView 包裹可刷新内容并以 IsRefreshing/Command 驱动。",
                "CarouselView, SwipeView, RefreshView"),
            new(LearningLevel.Intermediate, "BindableLayout 边界",
                "少量动态子项可用 BindableLayout；它不虚拟化。多类型卡片通过 DataTemplateSelector 选择外观，模型工厂与模板选择职责分开。",
                "Widget hierarchy + WidgetTemplateSelector"),
            new(LearningLevel.Intermediate, "Grid 与 StackLayout",
                "Grid 适合二维对齐和星号/自动尺寸；Vertical/HorizontalStackLayout 适合单轴流。避免用深层嵌套 Stack 模拟表格。",
                "Grid for alignment; StackLayout for one axis"),
            new(LearningLevel.Intermediate, "Flex 与 AbsoluteLayout",
                "FlexLayout 适合换行标签/工具条；AbsoluteLayout 适合覆盖、画布定位和比例区域，不适合作为普通表单的坐标系统。",
                "layout selection is based on constraints"),
            new(LearningLevel.Advanced, "Measure / Arrange 两阶段",
                "Measure 在约束下收集子项期望尺寸，Arrange 把最终 Rect 分配给子项。Measure 不应写绑定状态或触发再次布局。",
                "TwoColumnLayoutManager.Measure and ArrangeChildren"),
            new(LearningLevel.Advanced, "自定义 ILayoutManager",
                "自定义 Layout 通过 CreateLayoutManager 返回算法对象；布局参数用 BindableProperty，变化时 InvalidateMeasure。",
                "TwoColumnLayout + TwoColumnLayoutManager"),
            new(LearningLevel.Advanced, "无限约束",
                "ScrollView 等父级可能提供 Infinity；算法需要可解释回退和非负列宽，不能让 NaN/Infinity 传播到 Arrange。",
                "Measure uses 400 fallback and Math.Max"),
            new(LearningLevel.Advanced, "资源管道",
                "AppIcon、Splash、Fonts、Images 与 Raw 走各自 MAUI Item。矢量资源在构建期生成目标密度资产，运行时不要加载远超显示尺寸的位图。",
                "Resources/AppIcon and Resources/Splash in csproj"),
            new(LearningLevel.Advanced, "安全区与软键盘",
                "MAUI 10 的 SafeAreaEdges 表达内容延伸边界；表单还要实测软键盘遮挡、滚动到焦点和返回键收键盘，不硬编码刘海高度。",
                "SafeAreaEdges is preferred over fixed top padding"),
            new(LearningLevel.Advanced, "窗口宽度自适应",
                "响应式判断应基于可用窗口宽度而非仅 DeviceIdiom。紧凑、常规、宽屏可切换列数、模板和导航形态。",
                "ControlsFactory.GetAdaptiveColumns"),
            new(LearningLevel.Expert, "桌面输入与菜单",
                "Windows/Mac 上补齐指针悬停、ToolTip、键盘焦点、快捷键、MenuBar、TitleView 和 ToolbarItems；触屏仍需足够命中区域。",
                "desktop capabilities augment, not replace, touch"),
            new(LearningLevel.Expert, "GraphicsView、WebView 与 Map",
                "GraphicsView 用 IDrawable 做轻量绘制；WebView/HybridWebView 需审查来源与消息边界；Map 需要额外包、密钥和平台权限，仓库未配置的依赖不强加。",
                "GraphicsView/WebView included; Map absent so ignored"),
            new(LearningLevel.Expert, "布局性能",
                "减少无意义嵌套、避免 Measure 中分配/副作用，列表模板保持轻量；用 MAUI ActivitySource/Meter 和阶段 18 工具验证而非凭感觉优化。",
                "TwoColumnLayoutManager has allocation-bounded passes"),
            new(LearningLevel.Expert, "废弃扫描",
                "扫描 Frame、ListView、TableView 和 RelativeLayout 等旧主线用法，迁移到 Border、CollectionView、Grid/Flex/自定义 Layout。",
                "rg \"<Frame|<ListView|RelativeLayout\" src/LearnMaui"),
        ];

        LearningLab[] labs =
        [
            new("创建控件矩阵", "实例化文档主线控件并输出类型，验证 API 在当前 MAUI 版本可用。",
                _ =>
                {
                    IReadOnlyList<View> controls = ControlsFactory.CreateControlMatrix();
                    return Task.FromResult(
                        $"共 {controls.Count} 个控件：\n" +
                        string.Join(", ", controls.Select(view => view.GetType().Name)));
                }),
            new("验证自适应断点", "按当前可用宽度策略计算手机、平板与宽屏列数。",
                _ => Task.FromResult(
                    $"390px → {ControlsFactory.GetAdaptiveColumns(390)} 列\n" +
                    $"800px → {ControlsFactory.GetAdaptiveColumns(800)} 列\n" +
                    $"1440px → {ControlsFactory.GetAdaptiveColumns(1440)} 列")),
            new("构造两列布局", "创建带 BindableProperty 的自定义 Layout 及不同高度子项。",
                _ =>
                {
                    TwoColumnLayout layout = new() { Padding = 12, Spacing = 8 };
                    layout.Add(new Border { HeightRequest = 80 });
                    layout.Add(new Border { HeightRequest = 120 });
                    layout.Add(new Border { HeightRequest = 60 });
                    layout.Add(new Border { HeightRequest = 100 });
                    return Task.FromResult(
                        $"子项：{layout.Count}，Spacing={layout.Spacing}，Padding={layout.Padding}");
                }),
            new("选择 Widget 模板", "让 DataTemplateSelector 按运行时模型类型选择文本、图表或图片模板。",
                _ =>
                {
                    Widget[] widgets =
                    [
                        new TextWidget("t", "hello"),
                        new ChartWidget("c", [1, 2, 3]),
                        new ImageWidget("i", "appicon.svg"),
                    ];
                    WidgetTemplateSelector selector = new();
                    int uniqueTemplates = widgets
                        .Select(widget => selector.SelectTemplate(widget, new Label()))
                        .Distinct()
                        .Count();
                    return Task.FromResult($"3 种模型选择出 {uniqueTemplates} 种模板。");
                }),
        ];

        return new LearningStage(
            5,
            "控件、布局、自适应与资源",
            "第3部分-阶段5-控件布局自适应资源.md",
            topics,
            labs);
    }
}
