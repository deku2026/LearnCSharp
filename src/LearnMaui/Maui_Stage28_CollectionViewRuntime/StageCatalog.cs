using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage28_CollectionViewRuntime;

internal static class StageCatalog
{
    public static LearningStage Create(
        CollectionLabViewModel viewModel,
        EssentialsProbe essentials)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "CollectionView 入口",
                "ItemsSource、ItemTemplate、ItemsLayout、selection、scroll 和 threshold 进入 ItemsView，再由 Handler 映射到平台虚拟化列表。",
                "MainPage CollectionView"),
            new(LearningLevel.Beginner, "通知契约",
                "集合增删替换依赖 INotifyCollectionChanged，项内容依赖 INotifyPropertyChanged；在后台线程直接改绑定集合不安全。",
                "ResettableObservableCollection + dispatcher"),
            new(LearningLevel.Beginner, "编译项绑定",
                "DataTemplate 自己声明 item x:DataType，让模板属性访问编译检查；父页面 ViewModel 类型不会自动适用于项。",
                "RuntimeSampleItem DataTemplate"),
            new(LearningLevel.Beginner, "平台虚拟化",
                "Android 通常落到 RecyclerView，Apple/Windows 使用各自列表；只创建/绑定可见附近项，模板重活仍会让滚动掉帧。",
                "CollectionPipeline"),
            new(LearningLevel.Intermediate, "稳定身份",
                "业务项持有稳定 Id；分页/刷新不要用索引当身份，选择、增量更新和自动化都依赖可重复定位。",
                "RuntimeSampleItem.Id"),
            new(LearningLevel.Intermediate, "分页背压",
                "RemainingItemsThreshold 只是信号；用 gate 合并重复请求、传播取消、判断 hasMore，并在 UI dispatcher 批量提交。",
                "LoadMoreAsync SemaphoreSlim"),
            new(LearningLevel.Intermediate, "刷新语义",
                "RefreshView 明确 loading 状态；刷新替换 SSOT 并复位页码，finally 关闭 spinner，错误不能留下永久刷新态。",
                "RefreshAsync"),
            new(LearningLevel.Intermediate, "滚动状态",
                "Scrolled 的 first/center/last 用于诊断和恢复；高频事件只记录轻量数据，节流遥测，避免每像素触发业务加载。",
                "VisibleRange"),
            new(LearningLevel.Intermediate, "尺寸策略",
                "MeasureFirstItem 仅适合等高模板；异高内容需要测量并关注成本。不要把 CollectionView 放进无界同方向 ScrollView。",
                "ItemSizingStrategy"),
            new(LearningLevel.Advanced, "模板热路径",
                "减少嵌套、Converter、阴影、大图解码和动态资源；复用不等于 ViewModel/事件可无限持有旧 item。",
                "compact compiled template"),
            new(LearningLevel.Advanced, "Essentials 门面",
                "DeviceInfo/Connectivity/Permissions/SecureStorage/Geolocation 是共享门面，内部按平台实现；共享 API 不保证能力完全对等。",
                "EssentialsProbe"),
            new(LearningLevel.Advanced, "Permissions",
                "先 CheckStatus，再在有上下文的用户操作中 Request；Denied/Restricted/Disabled/Unknown 都是正常结果，不循环弹窗。",
                "CheckStatusAsync only"),
            new(LearningLevel.Advanced, "SecureStorage",
                "加密存储用于小型凭据，不存大 JSON；备份/卸载/密钥变化会导致读失败，catch 后回到重新登录而不是泄露值。",
                "value-safe round-trip probe"),
            new(LearningLevel.Advanced, "Geolocation",
                "清单声明、运行时权限、系统定位开关、精度、超时、取消和缓存位置共同决定结果；无位置不是异常业务。",
                "GetLastKnownLocationAsync"),
            new(LearningLevel.Advanced, "Mono 与 CoreCLR",
                "Android/iOS 主要使用 Mono runtime 路径，Windows 常见 CoreCLR；JIT/AOT、GC、诊断和动态代码能力随平台不同。",
                "RuntimeInspector"),
            new(LearningLevel.Expert, "Linker/Trim/AOT",
                "SourceGen XAML/JSON、静态路由和明确元数据减少反射风险；RuntimeFeature 只能报告能力，Release publish 警告仍需分类验证。",
                "RuntimeSnapshot + SourceGen XAML"),
            new(LearningLevel.Expert, "Hot Reload 边界",
                "Hot Reload 是开发期增量修改，不代表冷启动、trim、AOT 或商店包行为；结构性变化需完整重启重建。",
                "runtime map"),
            new(LearningLevel.Expert, "Handler 调试链",
                "通知→ItemsView→CollectionViewHandler→RecyclerView/controller→item Handler/Layout；按层计数创建、绑定、测量和回收。",
                "CollectionPipeline"),
            new(LearningLevel.Expert, "内存与回收",
                "图片、事件、messenger、定时器和 item VM 可阻止回收；快速滚动后做 GC/弱引用实验并对照平台 profiler。",
                "RuntimeInspector managed bytes"),
            new(LearningLevel.Expert, "第 9 部分收官",
                "能串讲 Builder/DI、Bindable/XAML、Handler/Layout、Window/Shell/Dispatcher、CollectionView/Essentials/Runtime 五条调用链。",
                "pipeline labs + stages 24-28"),
        ];

        LearningLab[] labs =
        [
            new("执行分页背压", "连续调用分页协调器并查看合并后的稳定列表数量。",
                async token =>
                {
                    Task first = viewModel.LoadMoreAsync(token);
                    Task second = viewModel.LoadMoreAsync(token);
                    await Task.WhenAll(first, second);
                    return $"count={viewModel.Items.Count}; last={viewModel.LastOperation}";
                }),
            new("执行原子刷新", "刷新集合并复位分页状态，最终 spinner 必须关闭。",
                async token =>
                {
                    await viewModel.RefreshAsync(token);
                    return $"count={viewModel.Items.Count}; refreshing={viewModel.IsRefreshing}; {viewModel.LastOperation}";
                }),
            new("查看可见范围", "输出真实 CollectionView 最近一次滚动窗口。",
                unusedToken =>
                {
                    VisibleRange? range = viewModel.VisibleRange;
                    return Task.FromResult(range is null
                        ? "Scroll the list to capture first/center/last."
                        : $"first={range.First}; center={range.Center}; last={range.Last}; delta={range.VerticalDelta:0.##}");
                }),
            new("抽样 Essentials", "检查权限、设备、网络、SecureStorage 和最后位置；不请求新权限、不输出 secret。",
                async token =>
                {
                    EssentialsSnapshot snapshot = await essentials.InspectAsync(token);
                    return $"platform={snapshot.Platform}; idiom={snapshot.Idiom}; device={snapshot.Device}\n" +
                        $"network={snapshot.NetworkAccess}; location permission={snapshot.LocationPermission}\n" +
                        $"secure round-trip={snapshot.SecureStorageRoundTrip}; last location={snapshot.LastKnownLocation}";
                }),
            new("捕获 Runtime", "输出运行时、架构、动态代码、GC 和 managed memory 快照。",
                unusedToken =>
                {
                    RuntimeSnapshot snapshot = RuntimeInspector.Capture();
                    return Task.FromResult(
                        $"{snapshot.Framework}; flavor={snapshot.RuntimeFlavor}; arch={snapshot.ProcessArchitecture}\n" +
                        $"dynamic supported={snapshot.DynamicCodeSupported}; compiled={snapshot.DynamicCodeCompiled}\n" +
                        $"serverGC={snapshot.ServerGc}; gen={snapshot.GcGeneration}; managed={snapshot.ManagedBytes:N0}\n" +
                        $"tfm={snapshot.Tfm}");
                }),
            new("打印列表调用链", "输出通知到 Android RecyclerView/原生项的六层路径。",
                unusedToken => Task.FromResult(string.Join(
                    '\n',
                    CollectionPipeline.Describe().Select(step =>
                        $"{step.Order}. {step.Layer}: {step.Responsibility}")))),
        ];

        return new LearningStage(
            28,
            "CollectionView Handler、Essentials 与 Runtime",
            "第9部分-05-CollectionView-Essentials-Runtime.md",
            topics,
            labs);
    }
}
