using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage31_RenderingExpert.Pages;

namespace LearnAvalonia.Avalonia_Stage31_RenderingExpert.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 Retained-mode", "第11部分-阶段31-渲染专家档.md §1", "解释 retained 与对开发者含义", () => new RetainedModeDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 Scene / Composition", "第11部分-阶段31-渲染专家档.md §2", "画出 composition 角色", () => new SceneCompositionDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 Dirty Region", "第11部分-阶段31-渲染专家档.md §3", "解释失效与重绘边界", () => new DirtyRegionDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 Hit Testing", "第11部分-阶段31-渲染专家档.md §4", "演示命中测试非矩形", () => new HitTestingDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 GPU 上下文", "第11部分-阶段31-渲染专家档.md §5", "列出失败模式与切换", () => new GpuContextDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 Skia 主线", "第11部分-阶段31-渲染专家档.md §6", "画出 Draw 调用栈", () => new SkiaMainDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 Metal", "第11部分-阶段31-渲染专家档.md §7", "macOS Metal 角色", () => new MetalDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 OpenGL / EGL", "第11部分-阶段31-渲染专家档.md §8", "列出 GL 入口", () => new OpenGlDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 CanvasKit (Browser)", "第11部分-阶段31-渲染专家档.md §9", "WASM CanvasKit 说明", () => new CanvasKitDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 软件渲染", "第11部分-阶段31-渲染专家档.md §10", "解释降级与取舍", () => new SoftwareRenderingDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 刷新率/帧时序", "第11部分-阶段31-渲染专家档.md §11", "列出 12.1 变化", () => new RefreshRateDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 UI vs 渲染线程", "第11部分-阶段31-渲染专家档.md §12", "解释同步与可取消", () => new ThreadsDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 GPU 失效恢复", "第11部分-阶段31-渲染专家档.md §13", "列出降级行为", () => new GpuLossDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 Overdraw / 关键字限制", "第11部分-阶段31-渲染专家档.md §14-15", "解释 overdraw 红线", () => new OverdrawDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 自定义 Render 示例", "第11部分-阶段31-渲染专家档.md §16", "写 Control+Render 自绘", () => new CustomRenderDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§16 Impeller (🔴 跟踪)", "第11部分-阶段31-渲染专家档.md §18", "标记实验，不生产", () => new ImpellerDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§17 Direct2D1 移除 / 平台表", "第11部分-阶段31-渲染专家档.md §19-20", "列出 12.1 渲染表", () => new PlatformRenderingTableDemo()) { Order = 17 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
