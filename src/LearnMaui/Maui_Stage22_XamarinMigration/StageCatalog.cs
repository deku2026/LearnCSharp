using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage22_XamarinMigration;

internal static class StageCatalog
{
    public static LearningStage Create(MigrationAuditor auditor)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "迁移不是复制",
                "先盘点页面、包、Renderer/Effect、平台服务和发布链，再按可回滚垂直切片迁移；旧项目只作行为基线，不复制技术债。",
                "MigrationPlanner M0-M7"),
            new(LearningLevel.Beginner, "单项目骨架",
                "SDK-style MAUI SingleProject 统一资源与平台入口；旧头项目文件、packages.config 和 AssemblyInfo 不直接搬运。",
                "current MAUI csproj"),
            new(LearningLevel.Beginner, "命名空间",
                "Xamarin.Forms 替换为 Microsoft.Maui.Controls，Xamarin.Essentials 合入 Microsoft.Maui.*；Android.Support 迁 AndroidX。",
                "MigrationAuditor rules"),
            new(LearningLevel.Beginner, "Frame → Border",
                "Border 提供 Stroke、StrokeShape 与 Content；不要继续复制已过时的 Frame 阴影/圆角心智。",
                "ModernMigrationSlice"),
            new(LearningLevel.Intermediate, "ListView → CollectionView",
                "迁移 ItemsSource、DataTemplate、选择和空态；Cell/ViewCell/TableView 不再进入新代码，先测回收与大列表性能。",
                "audit rule + shared CollectionView stage UI"),
            new(LearningLevel.Intermediate, "Renderer → Handler",
                "先识别属性映射和原生事件，再用 Handler/Mapper 替换；ConnectHandler 订阅、DisconnectHandler 对称解绑。",
                "blocker audit rule and stage 26 handoff"),
            new(LearningLevel.Intermediate, "Effect 去向",
                "纯视觉优先 Style/Behavior/VSM，手势用 GestureRecognizer，平台属性才进 Handler mapper；避免把 Effect 原样包一层。",
                "modern replacement decision"),
            new(LearningLevel.Intermediate, "DependencyService → DI",
                "接口放共享层，平台实现从 MauiProgram 注册并构造注入；禁止服务定位与静态容器。",
                "MauiProgram constructor injection"),
            new(LearningLevel.Intermediate, "MessagingCenter 清理",
                "用强类型事件、WeakReferenceMessenger 或显式状态流；订阅关系要有 owner、生命周期与可回收测试。",
                "blocker audit rule"),
            new(LearningLevel.Intermediate, "启动链迁移",
                "Forms.Init 与平台 MainActivity 手工装配迁到 MauiAppBuilder；字体、Handler、生命周期、配置和服务都集中在 composition root。",
                "MauiProgram"),
            new(LearningLevel.Advanced, "Shell 与路由",
                "先建立 route 常量、参数 DTO、auth gate 和深链回归，再迁页面；不要让旧 NavigationPage 调用散落进 ViewModel。",
                "M2 plan"),
            new(LearningLevel.Advanced, "异步 API",
                "DisplayAlert/FadeTo 等同步旧名迁到 Async 版本；扫描结果需人工区分注释和真正调用，不做盲目字符串替换。",
                "review-severity rules"),
            new(LearningLevel.Advanced, "平台分支",
                "Device.RuntimePlatform 分支迁到 DeviceInfo 或平台服务；#if 留在头项目/平台边缘，Core 不感知平台。",
                "audit + dependency boundary"),
            new(LearningLevel.Advanced, "插件审计",
                "每个 Xamarin 插件核对 MAUI 支持、权限、生命周期、trim/AOT 与维护状态；已有 MAUI API 时优先移除插件。",
                "M5 exit evidence"),
            new(LearningLevel.Advanced, "禁令扫描",
                "关键词扫描是快速失败门，不是语义分析器；csproj/包引用和同步 API 要专项检查并人工复核。",
                "MigrationAuditor regex timeouts"),
            new(LearningLevel.Advanced, "批次回归",
                "每批运行启动、导航返回、旋转/恢复、权限拒绝、离线、无障碍、性能与 Release smoke，并保留前后证据。",
                "MigrationBatch ExitEvidence"),
            new(LearningLevel.Expert, "兼容垫片有期限",
                "Compatibility 只能作为短期桥；每个 shim 必须有 owner、引入日、删除日、用量指标和失败时回滚方案。",
                "CompatibilityShim"),
            new(LearningLevel.Expert, "消融迁移",
                "替换 Renderer 后删除注册与旧实现，确认行为仍在；双实现长期共存会让缺陷来源和生命周期不可判定。",
                "M4 rollback-ready batch"),
            new(LearningLevel.Expert, "发布门",
                "Debug 迁移成功不等于完成；还需真机 Release、trim 警告分类、签名安装、升级数据保留与商店包 smoke。",
                "M7 plan"),
            new(LearningLevel.Expert, "完成定义",
                "旧包/命名空间/禁令为零，所有批次有证据，垫片清零或有到期单，性能/无障碍/发布无回退，才算迁移完成。",
                "auditor + plan + shim expiry"),
        ];

        LearningLab[] labs =
        [
            new("运行禁令扫描", "扫描一段遗留代码并输出行号、级别和现代替换。",
                unusedToken =>
                {
                    const string legacy = """
                        using Xamarin.Forms;
                        var page = new ListView();
                        DependencyService.Get<ILocation>();
                        MessagingCenter.Send(this, "refresh");
                        """;
                    IReadOnlyList<MigrationFinding> findings = auditor.Scan(legacy);
                    return Task.FromResult(string.Join(
                        '\n',
                        findings.Select(f =>
                            $"L{f.Line} [{f.Severity}] {f.LegacyPattern} -> {f.ModernReplacement}")));
                }),
            new("检查迁移波次", "生成 M0-M7 增量迁移顺序与退出证据。",
                unusedToken => Task.FromResult(string.Join(
                    '\n',
                    MigrationPlanner.CreateDefaultPlan().Select(
                        batch => $"{batch.Wave}: {batch.ExitEvidence}; rollback={batch.RollbackReady}")))),
            new("验证垫片到期", "演示 Compatibility shim 的 owner 与硬删除日期。",
                unusedToken =>
                {
                    CompatibilityShim shim = new(
                        "LegacyLocationAdapter",
                        new DateOnly(2026, 1, 10),
                        new DateOnly(2026, 3, 31),
                        "mobile-platform");
                    return Task.FromResult(
                        $"{shim.Name}, owner={shim.Owner}, expired={shim.IsExpired(DateOnly.FromDateTime(DateTime.Today))}");
                }),
            new("构造现代 UI 切片", "验证 Border 与 TapGestureRecognizer 的现代替换对象。",
                unusedToken =>
                {
                    View view = ModernMigrationSlice.Create();
                    Border border = (Border)view;
                    Label label = (Label)border.Content!;
                    return Task.FromResult(
                        $"root={border.GetType().Name}, child={label.GetType().Name}, gestures={label.GestureRecognizers.Count}");
                }),
            new("输出批次回归表", "列出每一迁移批次不可省略的回归面。",
                unusedToken => Task.FromResult(
                    "startup | navigation/back/deep-link | rotation/resume | permission denied | offline\n" +
                    "accessibility | collection virtualization | release trim/sign/install | upgrade data")),
        ];

        return new LearningStage(
            22,
            "Xamarin.Forms 迁移清理地图",
            "第8部分-阶段22-Xamarin-Forms迁移清理地图.md",
            topics,
            labs);
    }
}
