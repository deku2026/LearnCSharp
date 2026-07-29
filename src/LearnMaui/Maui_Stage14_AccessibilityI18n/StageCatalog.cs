using System.Globalization;
using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage14_AccessibilityI18n;

internal static class StageCatalog
{
    public static LearningStage Create(LocalizedText localizedText)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "语义树而非像素",
                "辅助技术读取角色、名称、值、状态与操作。视觉上明显的图标/颜色对读屏可能毫无意义，必须提供语义等价。",
                "AccessibilityDemoView SemanticProperties"),
            new(LearningLevel.Beginner, "Description 与 Hint",
                "Description 回答“是什么/当前值”，Hint 回答“操作后会怎样”；不要重复按钮可见文字，也不要把长帮助塞进每次聚焦。",
                "Entry and Button semantic properties"),
            new(LearningLevel.Beginner, "Heading 层级",
                "Level1 是页面主标题，Level2/3 形成连续结构，读屏用户可按标题导航；不能只为放大字体滥用 Heading。",
                "Title Level1 + status section Level2"),
            new(LearningLevel.Beginner, "SemanticScreenReader",
                "异步状态完成可 Announce 短消息而不抢焦点；防抖重复播报，错误信息同时保留可见文本。",
                "OnSaveClicked → Announce"),
            new(LearningLevel.Beginner, "AutomationId 分工",
                "AutomationId 是稳定测试定位符，SemanticProperties 是用户语义。ID 按业务目标命名，不按第几个 Grid 子项。",
                "Stage14NoteEntry / Stage14SaveButton"),
            new(LearningLevel.Intermediate, "TalkBack / VoiceOver / Narrator",
                "至少真机走聚焦顺序、名称、操作、返回、弹窗与动态更新；平台读屏手势不同，但共享语义目标一致。",
                "semantic tree is shared; verification is platform-specific"),
            new(LearningLevel.Intermediate, "对比度",
                "普通/大文本与非文本控件满足目标对比度，浅深主题都测；颜色不能是错误、选择或图表的唯一编码。",
                "text plus semantic status, not color-only state"),
            new(LearningLevel.Intermediate, "动态字体",
                "允许系统字体缩放，避免固定高度裁字；200% 缩放时表单可重排/滚动，关键按钮仍可达。",
                "FontAutoScalingEnabled + ScrollView"),
            new(LearningLevel.Intermediate, "触控目标与焦点",
                "主要操作至少提供足够命中区域，视觉图标小可扩展容器；焦点顺序跟任务一致，弹层关闭后返回触发点。",
                "MinimumHeightRequest=48"),
            new(LearningLevel.Intermediate, "减少动效",
                "动画不承载唯一信息，系统减少动态时跳过大位移/缩放；自动播放媒体允许暂停。",
                "accessibility policy gates Stage08 animation"),
            new(LearningLevel.Intermediate, "RTL 与双向文本",
                "根据文化切换 FlowDirection，使用 Start/End 语义并实测阿拉伯语/希伯来语夹数字与 URL；图标是否镜像按含义决定。",
                "LocalizedText.GetFlowDirection"),
            new(LearningLevel.Advanced, "RESX 资源",
                "neutral 与 culture-specific resx 使用稳定 key；代码/XAML 不硬编码用户文案，缺失键有可诊断回退。",
                "AppResources.resx + AppResources.zh-Hans.resx"),
            new(LearningLevel.Advanced, "文化格式",
                "日期、数字、货币使用 CurrentCulture；资源选择使用 CurrentUICulture。持久化与协议仍使用 invariant/ISO 形式。",
                "LocalizedText + CultureInfo"),
            new(LearningLevel.Advanced, "复数",
                "复数规则依语言而异，英语 one/many 只是最小示例；生产使用支持 CLDR/ICU 规则的本地化方案。",
                "FormatItemCount"),
            new(LearningLevel.Advanced, "应用内语言切换",
                "切换 culture 后通知可见 View 刷新资源、FlowDirection 和格式；偏好持久化并在建首窗前恢复，避免混合语言。",
                "CultureChanged event + AccessibilityDemoView.ApplyCulture"),
            new(LearningLevel.Advanced, "表单错误",
                "错误与字段关联、有可见文字和语义通知，焦点移到首个错误但不造成循环；星号/红色不能单独表示必填。",
                "semantic description and status surface"),
            new(LearningLevel.Advanced, "加载三态",
                "Loading 初次播报一次，Content 不逐项播报，Empty 给下一步，Error 给重试；焦点不因集合刷新任意跳走。",
                "state transition announcements, not spinner chatter"),
            new(LearningLevel.Expert, "平台体验适配",
                "共享功能遵循 Material/HIG/Windows 的导航、返回、菜单、触觉和快捷键惯例；不追求像素一致而破坏平台预期。",
                "OnPlatform/handlers only for meaningful adaptation"),
            new(LearningLevel.Expert, "媒体与地图",
                "图片提供等价文本或标记装饰；视频有字幕/音频描述/控制；地图信息提供列表/地址等非视觉替代。",
                "Image has SemanticProperties.Description"),
            new(LearningLevel.Expert, "测试矩阵",
                "自动化检查 ID、可访问名称、对比度只能辅助；手动读屏、键盘、字体 200%、RTL、长翻译和主题切换才是验收。",
                "AccessibilityDemoView is a stable manual/automation target"),
        ];

        LearningLab[] labs =
        [
            new("读取本地化字符串", "从 neutral/zh-Hans RESX 读取页面标题和按钮文字。",
                _ => Task.FromResult(
                    $"culture={CultureInfo.CurrentUICulture.Name}\n" +
                    $"title={localizedText["PageTitle"]}\nsave={localizedText["Save"]}")),
            new("切换中英文", "在 en 与 zh-Hans 间切换并通知可见 View 刷新。",
                _ =>
                {
                    string next = CultureInfo.CurrentUICulture.Name.StartsWith(
                        "zh",
                        StringComparison.OrdinalIgnoreCase)
                        ? "en"
                        : "zh-Hans";
                    localizedText.SetCulture(next);
                    return Task.FromResult(
                        $"culture={CultureInfo.CurrentUICulture.Name}, title={localizedText["PageTitle"]}");
                }),
            new("验证文化格式与复数", "按当前文化格式化日期、货币和 1/3 项。",
                _ => Task.FromResult(
                    $"date={DateTimeOffset.Now:d}\n" +
                    $"currency={1234.56:C}\n" +
                    $"{localizedText.FormatItemCount(1)} / {localizedText.FormatItemCount(3)}")),
            new("构造无障碍 View", "实例化包含 heading、hint、AutomationId、动态字体与触控目标的 XAML。",
                _ =>
                {
                    AccessibilityDemoView view = new(localizedText);
                    return Task.FromResult(
                        $"view={view.GetType().Name}, flow={view.FlowDirection}, automation={view.AutomationId}");
                }),
            new("读屏播报", "通过 SemanticScreenReader 播报短状态，不移动当前焦点。",
                _ =>
                {
                    string message = localizedText["SavedAnnouncement"];
                    SemanticScreenReader.Default.Announce(message);
                    return Task.FromResult($"Announced: {message}");
                }),
        ];

        return new LearningStage(
            14,
            "无障碍、本地化与平台体验",
            "第5部分-阶段14-无障碍本地化平台体验.md",
            topics,
            labs);
    }
}
