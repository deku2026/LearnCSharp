using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage12_InputFocusGestures.Pages;

namespace LearnAvalonia.Avalonia_Stage12_InputFocusGestures.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§1 为什么输入是专家分水岭", "第5部分-阶段12-输入焦点手势与IME.md §1", "区分按键/指针/手势/IME 四通道", () => new StageOverviewDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§2.1 路由事件：direct/tunnel/bubble", "第5部分-阶段12-输入焦点手势与IME.md §2.1", "演示 Handled 与 handledEventsToo", () => new RoutedEventDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§2.2 指针：mouse/touch/pen", "第5部分-阶段12-输入焦点手势与IME.md §2.2", "Pointer.Capture 与 CaptureLost", () => new PointerDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§2.3 手势事件在 InputElement", "第5部分-阶段12-输入焦点手势与IME.md §2.3", "Tapped/DoubleTapped/Holding 实例事件", () => new GesturesDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§2.4 键盘与热键", "第5部分-阶段12-输入焦点手势与IME.md §2.4", "KeyModifiers + KeyGesture + HotKey", () => new KeyboardHotkeyDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§2.5 焦点：scopes、Tab、FocusManager", "第5部分-阶段12-输入焦点手势与IME.md §2.5", "Focus/IsFocusScope/GotFocus 导航方式", () => new FocusDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§2.6 Access Keys（按符号）", "第5部分-阶段12-输入焦点手势与IME.md §2.6", "Alt+字符 按符号匹配与降级", () => new AccessKeysDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§2.7 IME 与文本输入", "第5部分-阶段12-输入焦点手势与IME.md §2.7", "TextInput 上屏 + 组合期 Enter", () => new ImeDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§2.8 拖放 DnD 与 IAsyncDataTransfer", "第5部分-阶段12-输入焦点手势与IME.md §2.8", "DoDragDropAsync + DataTransfer", () => new DragDropDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§2.9 剪贴板（IAsyncDataTransfer）", "第5部分-阶段12-输入焦点手势与IME.md §2.9", "TryGetTextAsync 取代 GetTextAsync", () => new ClipboardDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§2.10 PlatformFeedback（触觉/声音）", "第5部分-阶段12-输入焦点手势与IME.md §2.10", "移动触觉需提供视觉替代", () => new PlatformFeedbackDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§2.11 浏览器限制", "第5部分-阶段12-输入焦点手势与IME.md §2.11", "剪贴板/热键/捕获在浏览器门控", () => new BrowserLimitsDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§2.12 与命令系统集成", "第5部分-阶段12-输入焦点手势与IME.md §2.12", "HotKey+Command + 隧道转发", () => new CommandIntegrationDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§2.13 调试输入的方法", "第5部分-阶段12-输入焦点手势与IME.md §2.13", "临时 handler/DevTools/CaptureLost", () => new DebugInputDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§3 跨平台差异", "第5部分-阶段12-输入焦点手势与IME.md §3", "右键/全局热键/IME/Meta 键差异", () => new PlatformDiffDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§4 Avalonia 12 / 12.1 变化", "第5部分-阶段12-输入焦点手势与IME.md §4", "手势迁 InputElement/异步 DnD", () => new Avalonia12ChangesDemo()) { Order = 16 });
        Topics.Add(new TopicItem("§5 过时与禁止", "第5部分-阶段12-输入焦点手势与IME.md §5", "禁旧 Gestures 静态/同步 DnD", () => new DeprecatedApisDemo()) { Order = 17 });
        Topics.Add(new TopicItem("§6 最小实验清单", "第5部分-阶段12-输入焦点手势与IME.md §6", "完成路由/捕获/手势/IME 4 实验", () => new MinExperimentsDemo()) { Order = 18 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
