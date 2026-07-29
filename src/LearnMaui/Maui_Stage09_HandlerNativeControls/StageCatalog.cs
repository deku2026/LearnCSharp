using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage09_HandlerNativeControls;

internal static class StageCatalog
{
    public static LearningStage Create()
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "Virtual View",
                "Entry、Button 等跨平台控件是虚拟视图，保存可绑定属性和语义，不直接继承 Android/iOS 原生控件。",
                "TintedEntry : Entry"),
            new(LearningLevel.Beginner, "Handler",
                "Handler 连接 VirtualView 与 PlatformView，拥有 PropertyMapper/CommandMapper、MauiContext 和连接/断开生命周期。",
                "AuditedEntryHandler : EntryHandler"),
            new(LearningLevel.Beginner, "Platform View",
                "Android Entry 对应 EditText，iOS 对应 UITextField，Windows 对应 TextBox。平台对象只在 Handler 已连接时访问。",
                "handler.PlatformView under #if ANDROID"),
            new(LearningLevel.Beginner, "属性更新链",
                "SetValue 触发 BindableProperty 变化，Element 请求 Handler.UpdateValue，Mapper 找到键并把新值写到原生控件。",
                "TintColorProperty propertyChanged → Handler.UpdateValue"),
            new(LearningLevel.Beginner, "PropertyMapper",
                "PropertyMapper 把属性名映射成更新委托。AppendToMapping 在框架映射后追加逻辑；键必须命名唯一避免库间覆盖。",
                "EntryHandler.Mapper.AppendToMapping"),
            new(LearningLevel.Intermediate, "CommandMapper",
                "CommandMapper 映射无持久值的动作，如聚焦、滚动或平台命令；属性状态继续用 PropertyMapper，避免语义混用。",
                "IElementHandler.Invoke dispatches mapped commands"),
            new(LearningLevel.Intermediate, "全局定制",
                "修改 EntryHandler.Mapper 会影响进程内所有 Entry，适合统一选择策略等真正全局规范，必须做全应用回归。",
                "GlobalEntryKey sets Android select-all-on-focus"),
            new(LearningLevel.Intermediate, "控件类型定制",
                "自定义 TintedEntry 注册专用 Handler，只影响该控件类型，是产品组件的首选范围。",
                "AddHandler<TintedEntry, AuditedEntryHandler>"),
            new(LearningLevel.Intermediate, "单实例定制",
                "偶发页面特例可在 HandlerChanged 后修改当前 PlatformView，并在 HandlerChanging/Disappearing 撤销；不要升级成全局副作用。",
                "HandlerLifetimeProbe Attach/Detach"),
            new(LearningLevel.Intermediate, "Connect / Disconnect 成对",
                "原生事件订阅、观察器和委托在 ConnectHandler 建立，在 DisconnectHandler 对称解除；否则页面离开后 Handler 会泄漏。",
                "AuditedEntryHandler FocusChange += / -="),
            new(LearningLevel.Intermediate, "Platforms 与 partial",
                "大段平台实现放 Platforms/Android 或 partial 文件；小型类型引用可用 #if。共享接口保持无 Android 类型，方便测试与多目标。",
                "#if ANDROID isolates EditText and ColorStateList"),
            new(LearningLevel.Advanced, "Mapper 执行时机",
                "Handler 建立时会映射初值，属性改变时按键再次更新；AppendToMapping 不是只运行一次的启动钩子，因此逻辑必须幂等、快速。",
                "GlobalMapperRuns counter demonstrates repeated execution"),
            new(LearningLevel.Advanced, "Prepend / Append / Modify",
                "Prepend 在框架前，Append 在框架后，Modify 包装旧映射。全局替换要保留旧逻辑并记录退出策略，避免破坏第三方控件。",
                "this project deliberately uses AppendToMapping"),
            new(LearningLevel.Advanced, "反馈循环",
                "原生 TextChanged 回写 VirtualView 时要比较新旧值并避免再次触发格式化循环；光标/IME 组合输入也必须保留。",
                "native events should call SetValue only when changed"),
            new(LearningLevel.Advanced, "Handler 与 Behavior",
                "Behavior 适合跨平台 View 行为；Handler 适合原生创建/映射；平台特定简单属性可用平台 API；Renderer 是迁移遗留，不进新主线。",
                "scope decision precedes implementation"),
            new(LearningLevel.Advanced, "不要缓存 PlatformView",
                "PlatformView 归 Handler 生命周期所有，Disconnect 后引用失效。服务/VM 不保存原生控件，页面只在已连接窗口内短暂访问。",
                "HandlerLifetimeProbe stores only a diagnostic type string"),
            new(LearningLevel.Expert, "自定义 Handler 与 DI",
                "Handler 可通过 MauiContext.Services 解析真正的平台桥接服务，但优先构造/属性显式传递；不要把 Mapper 变成服务定位器。",
                "handler.MauiContext.Services is an expert boundary"),
            new(LearningLevel.Expert, "布局与 Handler 边界",
                "共享 LayoutManager 负责 measure/arrange，Handler 负责把最终布局传给平台；普通自定义布局不需要改 Handler。",
                "layout algorithm remains outside EntryHandler"),
            new(LearningLevel.Expert, "CollectionView 源码阅读",
                "沿 ItemsView → Handler → Adapter/ViewHolder/平台集合读虚拟化、选择和滚动命令；区分框架 bug 与错误模板/集合更新。",
                "source path exercise; project has no custom collection handler"),
            new(LearningLevel.Expert, "调试与测试",
                "记录 VirtualView/Handler/PlatformView 类型、Connect/Disconnect 次数和 Mapper 键；设备测试原生行为，纯逻辑测试 BindableProperty。",
                "AuditedEntryHandler counters + HandlerLifetimeProbe"),
        ];

        LearningLab[] labs =
        [
            new("创建自定义控件", "创建 TintedEntry，更新 BindableProperty 并检查虚拟层状态。",
                _ =>
                {
                    TintedEntry entry = new()
                    {
                        Text = "Handler lab",
                        TintColor = Colors.MediumPurple,
                    };
                    return Task.FromResult(
                        $"VirtualView={entry.GetType().Name}, Tint={entry.TintColor.ToHex()}");
                }),
            new("检查 Android 原生类型", "输出当前目标平台中 Entry Handler 对应的原生控件类型。",
                _ =>
                {
#if ANDROID
                    return Task.FromResult(typeof(Android.Widget.EditText).FullName ?? "unknown");
#else
                    return Task.FromResult("This build is not Android.");
#endif
                }),
            new("验证事件所有权", "Attach/Detach 单实例探针并确认解除后不保留 View 引用。",
                _ =>
                {
                    Entry entry = new();
                    using HandlerLifetimeProbe probe = new();
                    probe.Attach(entry);
                    string attached = probe.PlatformViewType;
                    probe.Detach();
                    return Task.FromResult($"attached={attached}\nafter detach={probe.PlatformViewType}");
                }),
            new("查看 Handler 计数", "读取全局 Mapper 与自定义 Handler 连接/断开计数。",
                _ => Task.FromResult(
                    $"Mapper runs={HandlerCustomization.GlobalMapperRuns}\n" +
                    $"Connected={AuditedEntryHandler.ConnectedCount}\n" +
                    $"Disconnected={AuditedEntryHandler.DisconnectedCount}")),
        ];

        return new LearningStage(
            9,
            "Handler、Mapper 与原生控件",
            "第4部分-阶段9-Handler与原生控件.md",
            topics,
            labs);
    }
}
