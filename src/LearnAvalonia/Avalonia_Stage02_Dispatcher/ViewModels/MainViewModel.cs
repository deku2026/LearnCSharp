using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage02_Dispatcher.Pages;

namespace LearnAvalonia.Avalonia_Stage02_Dispatcher.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 UI 线程纪律", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §1", "说明事故成因与 lifetime 关系", () => new UiThreadDisciplineDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2 Dispatcher 家族 API", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §2", "讲清 UIThread/CurrentDispatcher/AvaloniaObject.Dispatcher", () => new DispatcherFamilyDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§3 Post/InvokeAsync/优先级", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §3", "说明 Post 与 InvokeAsync 差异及 ExecutionContext 捕获", () => new PostInvokeAsyncDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§4 Yield/Resume/SyncContext", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §4", "理解 AvaloniaSynchronizationContext 与 ConfigureAwait", () => new YieldResumeDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§5 多 Dispatcher 单 UI 线程", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §5", "默写每线程 Dispatcher；多 UI 线程不支持", () => new MultiDispatcherDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§6 后台→UI 正误写法", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §6", "给出正确与错误示例各一", () => new BackgroundToUiDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§7 剪贴板死锁禁区", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §7", "列出剪贴板回调禁同步 Invoke", () => new ClipboardDeadlockDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§8 async/await 死锁场景", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §8", "列出至少 3 种死锁/卡顿禁令", () => new AsyncDeadlockDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§9 产品级异步 UI 模式", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §9", "可取消加载 + 节流搜索", () => new ProductAsyncPatternDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§10 跨平台差异表", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §10", "覆盖 7 类宿主线程模型", () => new PlatformThreadDiffDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§11 Avalonia 12 最新变化", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §11", "列出 12/12.1 Dispatcher 关键变化", () => new Dispatcher12ChangesDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§12 过时与禁止", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §12", "点名 8 项禁令与替代", () => new DispatcherDeprecatedDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§13 最小实验 A-F", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §13", "完成 6 个最小实验", () => new DispatcherExperimentsDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§14 源码阅读路径", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §14", "列出关键类型与关注点", () => new SourceReadingPathDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§15 验收标准", "第2部分-阶段2-Dispatcher线程与UI亲和性.md §15", "勾选 9 条验收清单", () => new DispatcherAcceptanceDemo()) { Order = 15 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
