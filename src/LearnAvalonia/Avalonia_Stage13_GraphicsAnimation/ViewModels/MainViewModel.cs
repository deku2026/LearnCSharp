using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage13_GraphicsAnimation.Pages;

namespace LearnAvalonia.Avalonia_Stage13_GraphicsAnimation.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 为什么图形与动画单独成章", "第5部分-阶段13-图形动画与效果.md §1", "区分即时模式与保留模式", () => new StageOverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 Brush 族", "第5部分-阶段13-图形动画与效果.md §2.1", "Solid/Linear/Radial/Image/Visual", () => new BrushDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 Geometry / Path / Shape", "第5部分-阶段13-图形动画与效果.md §2.2", "StreamGeometry/PathGeometry/Shape", () => new GeometryShapeDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 DrawingContext（即时模式）", "第5部分-阶段13-图形动画与效果.md §2.3", "AffectsRender + Render 纪律", () => new DrawingContextDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 位图", "第5部分-阶段13-图形动画与效果.md §2.4", "Bitmap/RenderTargetBitmap/Image", () => new BitmapDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 SVG 生态（选学前查）", "第5部分-阶段13-图形动画与效果.md §2.5", "11.x 包不硬上 12；图标优先 Path", () => new SvgEcosystemDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§2.6 Transforms", "第5部分-阶段13-图形动画与效果.md §2.6", "RenderTransform 不影响布局", () => new TransformsDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§2.7 Clip / Opacity / OpacityMask", "第5部分-阶段13-图形动画与效果.md §2.7", "ClipToBounds/Clip 几何/OpacityMask", () => new ClipOpacityDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§2.8 Effects / 阴影", "第5部分-阶段13-图形动画与效果.md §2.8", "DropShadowEffect 性能敏感", () => new EffectsDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§2.9 动画：Keyframe", "第5部分-阶段13-图形动画与效果.md §2.9", "Style.Animations + KeyFrame Cue", () => new KeyframeAnimationDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§2.10 Transitions", "第5部分-阶段13-图形动画与效果.md §2.10", "TransformOperations/Brush/Double", () => new TransitionsDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§2.11 Easing", "第5部分-阶段13-图形动画与效果.md §2.11", "CubicEaseOut/SineEaseInOut/Spring", () => new EasingDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§2.12 PlaybackBehavior 与不可见停动画", "第5部分-阶段13-图形动画与效果.md §2.12", "Infinite 动画离开视口治理", () => new PlaybackBehaviorDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§2.13 页面过渡", "第5部分-阶段13-图形动画与效果.md §2.13", "CrossFade/PageSlide 与控件分层", () => new PageTransitionDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§2.14 HarfBuzz 文本 shaping", "第5部分-阶段13-图形动画与效果.md §2.14", "复杂文种 shaping 验证", () => new HarfBuzzShapingDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§2.15 组合示例：可动画仪表", "第5部分-阶段13-图形动画与效果.md §2.15", "Gauge+AffectsRender+Transition", () => new AnimatedGaugeDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§3 跨平台差异", "第5部分-阶段13-图形动画与效果.md §3", "Skia/模糊/字体/DPI 差异", () => new PlatformDiffDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§4 Avalonia 12 / 12.1 变化", "第5部分-阶段13-图形动画与效果.md §4", "PlaybackBehavior/shaping 可配置", () => new Avalonia12ChangesDemo()) { Order = 18 });
        Topics.Add(new TopicItem("§5 过时与禁止", "第5部分-阶段13-图形动画与效果.md §5", "禁 Render 每帧 new/Infinite 不治理", () => new DeprecatedApisDemo()) { Order = 19 });
        Topics.Add(new TopicItem("§6 最小实验清单", "第5部分-阶段13-图形动画与效果.md §6", "完成 Shape/DrawingContext/动画 5 实验", () => new MinExperimentsDemo()) { Order = 20 });
        Topics.Add(new TopicItem("§10 性能配方", "第5部分-阶段13-图形动画与效果.md §10", "帧时序/设计资源/主题联动", () => new PerformanceRecipesDemo()) { Order = 21 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
