using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage26_HandlerMapperLayout;

internal static class StageCatalog
{
    public static LearningStage Create(TraceColumnLayout layout)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "虚拟视图与平台视图",
                "MAUI View 保存跨平台状态；Handler 持有 VirtualView、PlatformView 和 MauiContext，把属性/命令映射到原生控件。",
                "ExpertEntry + TracingEntryHandler"),
            new(LearningLevel.Beginner, "创建链",
                "元素进入可视树后解析 Handler，SetVirtualView/CreatePlatformView/ConnectHandler，映射初始属性并接入原生树。",
                "TracingEntryHandler.ConnectHandler"),
            new(LearningLevel.Beginner, "断开链",
                "移除/重建时 DisconnectHandler 必须对称解绑平台事件，再让 Handler/PlatformView 可回收；不能依赖终结器。",
                "FocusChange subscribe/unsubscribe"),
            new(LearningLevel.Beginner, "注册替换",
                "ConfigureMauiHandlers 把 ExpertEntry 精确注册到 TracingEntryHandler；全局替换基础 Entry 会扩大行为和回归面。",
                "MauiProgram handler registration"),
            new(LearningLevel.Intermediate, "PropertyMapper",
                "属性键映射 Action<THandler,TView>；BindableProperty 变化调用 Handler.UpdateValue，mapper 读取 VirtualView 写 PlatformView。",
                "AccentColor mapper"),
            new(LearningLevel.Intermediate, "CommandMapper",
                "CommandMapper 处理 Focus/SelectAll/Open 等动作，不表示持久属性；调用 Handler.Invoke(command,args) 触发平台操作。",
                "SelectAllCommand"),
            new(LearningLevel.Intermediate, "键一致性",
                "属性注册、UpdateValue 和 Mapper 必须使用同一 nameof/key；拼错通常静默不更新，因此键应常量化并有真机断言。",
                "AccentKey + nameof"),
            new(LearningLevel.Intermediate, "Prepend/Modify/Append",
                "Mapper 扩展是全局可组合行为；顺序和重复注册必须受控，配置幂等，并对非目标子类尽早返回。",
                "HandlerLabMappings.Configure guard"),
            new(LearningLevel.Intermediate, "平台隔离",
                "共享控件/属性留在公共代码，原生类型与 API 放平台条件或 partial 文件；Core 不引用 Android/iOS。",
                "ANDROID blocks"),
            new(LearningLevel.Advanced, "LayoutManager",
                "自定义 Layout 只拥有 children/BindableProperty，Measure/Arrange 算法下沉 ILayoutManager，便于单测约束和性能。",
                "TraceColumnLayoutManager"),
            new(LearningLevel.Advanced, "Measure",
                "Measure 接收可用约束并询问 child desired size；Infinity 表示父级未限制，不等于可以返回 Infinity 或 NaN。",
                "Measure normalization"),
            new(LearningLevel.Advanced, "Arrange",
                "Arrange 在最终 bounds 内给每个可见 child 确定 Rect；不要再次做昂贵数据工作，也不要修改 ItemsSource。",
                "ArrangeChildren"),
            new(LearningLevel.Advanced, "Padding/Spacing",
                "约束先扣 padding，children 之间仅加入 n-1 个 spacing，Collapsed 不占空间；返回尺寸要非负且有限。",
                "TraceColumnLayout calculations"),
            new(LearningLevel.Advanced, "Invalidation",
                "影响尺寸的 BindableProperty 变化调用 InvalidateMeasure；避免在滚动/绑定热路径形成整树 measure storm。",
                "Spacing property callback"),
            new(LearningLevel.Advanced, "密度与像素",
                "MAUI 布局用设备无关单位，Handler 最终映射到平台像素；Android/iOS 字体、density 和原生测量差异需分别记录。",
                "platform comparison topic"),
            new(LearningLevel.Expert, "诊断断点",
                "依次断 SetHandler/Connect、UpdateValue/mapper、native setter、Measure/Arrange；按层排除“值对但像素错”。",
                "MapperTrace + LayoutPass"),
            new(LearningLevel.Expert, "布局性能",
                "Measure/Arrange 热路径避免 LINQ、闭包和临时集合；缓存必须以约束/字体/内容变化正确失效，错误缓存比不缓存更危险。",
                "allocation-light loops"),
            new(LearningLevel.Expert, "CollectionView 接口",
                "列表卡顿先看模板层级、图片、绑定和测量次数，再看平台 Handler；不可为修一个 item 破坏全局 mapper。",
                "stage 28 handoff"),
            new(LearningLevel.Expert, "泄漏闭环",
                "平台事件、mapper 静态闭包、页面引用和长任务是经典根；事件 handler 不捕获 Page，Disconnect 后 connected/disconnected 可核对。",
                "static handler + counters"),
            new(LearningLevel.Expert, "验收",
                "能从 BindableProperty 追到 native setter，从父约束追到 child Rect，并在 Android 真机观察连接、命令和布局 pass。",
                "runnable handler/layout labs"),
        ];

        LearningLab[] labs =
        [
            new("查看 Handler 生命周期", "读取 Connect/Disconnect 计数和最近平台映射事件。",
                unusedToken => Task.FromResult(
                    $"connected={TracingEntryHandler.Connected}, disconnected={TracingEntryHandler.Disconnected}\n" +
                    string.Join('\n', MapperTrace.Snapshot()))),
            new("触发属性映射", "创建控件并更新 AccentColor，验证 UpdateValue 键和映射契约。",
                unusedToken =>
                {
                    ExpertEntry entry = new();
                    entry.AccentColor = Colors.Teal;
                    return Task.FromResult(
                        $"property={nameof(ExpertEntry.AccentColor)}, value={entry.AccentColor}, handler={entry.Handler?.GetType().Name ?? "created when attached"}");
                }),
            new("检查布局 Pass", "读取真实页面的 Measure/Arrange 约束记录。",
                unusedToken =>
                {
                    IReadOnlyList<LayoutPass> passes = layout.Snapshot();
                    return Task.FromResult(passes.Count == 0
                        ? "Layout pass occurs after attachment; run again after first frame."
                        : string.Join(
                            '\n',
                            passes.Select(pass =>
                                $"{pass.Kind}: {pass.Width:0.##} x {pass.Height:0.##}; children={pass.VisibleChildren}")));
                }),
            new("验证 Infinity 语义", "把未约束宽高归一化，确保宽度有策略且高度保持未限制。",
                unusedToken =>
                {
                    (double width, double height) = ConstraintLab.Normalize(
                        double.PositiveInfinity,
                        double.PositiveInfinity,
                        480);
                    return Task.FromResult($"width={width}; height infinite={double.IsInfinity(height)}");
                }),
            new("打印属性到平台链", "输出专家调试路径。",
                unusedToken => Task.FromResult(
                    "BindableProperty.SetValue\n" +
                    " -> Handler.UpdateValue(nameof(AccentColor))\n" +
                    " -> EntryHandler.Mapper.UpdateProperty\n" +
                    " -> Android MauiAppCompatEditText.BackgroundTintList\n" +
                    " -> native invalidation / next layout pass")),
        ];

        return new LearningStage(
            26,
            "Handler、Mapper 与 Layout",
            "第9部分-03-Handler-Mapper-Layout.md",
            topics,
            labs);
    }
}
