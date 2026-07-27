using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.ViewModels;

internal sealed partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private TopicItem? _selectedTopic;

    public ObservableCollection<TopicItem> Topics { get; } = [];

    public MainViewModel()
    {
        Topics.Add(new TopicItem("§A1-A2 DataContext 与绑定模式", "第4部分-阶段9-数据绑定与MVVM.md §A1", "解释 DataContext 继承与绑定模式", () => new DataContextBindingDemo()) { Order = 1 });
        Topics.Add(new TopicItem("§A3-A4 路径与 Fallback/TargetNull", "第4部分-阶段9-数据绑定与MVVM.md §A3", "使用 FallbackValue 与 TargetNullValue", () => new PathFallbackDemo()) { Order = 2 });
        Topics.Add(new TopicItem("§A5-A6 Converter 与 MultiBinding·诊断", "第4部分-阶段9-数据绑定与MVVM.md §A5", "使用 Converter 与绑定诊断", () => new ConverterMultiBindingDemo()) { Order = 3 });
        Topics.Add(new TopicItem("§B1-B2 Compiled vs Reflection", "第4部分-阶段9-数据绑定与MVVM.md §B1", "解释 compiled vs reflection 与 x:DataType", () => new CompiledReflectionDemo()) { Order = 4 });
        Topics.Add(new TopicItem("§B3-B4 代码侧绑定与何时 Reflection", "第4部分-阶段9-数据绑定与MVVM.md §B3", "说清 CompiledBinding.Create vs ReflectionBinding", () => new CodeBindingDemo()) { Order = 5 });
        Topics.Add(new TopicItem("§C1-C5 INPC·集合·命令", "第4部分-阶段9-数据绑定与MVVM.md §C1", "使用 Toolkit ObservableProperty 与 RelayCommand", () => new InpcCollectionCommandDemo()) { Order = 6 });
        Topics.Add(new TopicItem("§D1-D3 校验与 DataAnnotations", "第4部分-阶段9-数据绑定与MVVM.md §D1", "使用 ObservableValidator 与 INotifyDataErrorInfo", () => new ValidationDemo()) { Order = 7 });
        Topics.Add(new TopicItem("§E1-E2 VM 生命周期与 DI", "第4部分-阶段9-数据绑定与MVVM.md §E1", "写 DI 组合根注册 VM/服务", () => new VmLifecycleDiDemo()) { Order = 8 });
        Topics.Add(new TopicItem("§E3 Messaging", "第4部分-阶段9-数据绑定与MVVM.md §E3", "使用 WeakReferenceMessenger 收发消息", () => new MessagingDemo()) { Order = 9 });
        Topics.Add(new TopicItem("§E4 导航/对话框抽象", "第4部分-阶段9-数据绑定与MVVM.md §E4", "VM 只依赖接口不引用 Window", () => new NavigationDialogAbstractionDemo()) { Order = 10 });
        Topics.Add(new TopicItem("§F1-F2 Behaviors", "第4部分-阶段9-数据绑定与MVVM.md §F1", "说清 Behaviors 包与用途", () => new BehaviorsDemo()) { Order = 11 });
        Topics.Add(new TopicItem("§G1-G4 MVVM 框架选型表", "第4部分-阶段9-数据绑定与MVVM.md §G1", "默认方案口述：Toolkit+compiled+DI", () => new FrameworkSelectionDemo()) { Order = 12 });
        Topics.Add(new TopicItem("§H Avalonia 12 绑定变化", "第4部分-阶段9-数据绑定与MVVM.md §H", "说出绑定插件移除、DataAnnotations 默认关", () => new Avalonia12BindingChangesDemo()) { Order = 13 });
        Topics.Add(new TopicItem("§I 串联清单", "第4部分-阶段9-数据绑定与MVVM.md §I", "拼装 MainVM+服务+单测心智", () => new IntegrationChecklistDemo()) { Order = 14 });
        Topics.Add(new TopicItem("§J1-J3 平台差·过时·反模式", "第4部分-阶段9-数据绑定与MVVM.md §J1", "禁止推荐 Avalonia.ReactiveUI 11.x", () => new PlatformDeprecatedDemo()) { Order = 15 });
        Topics.Add(new TopicItem("§K1-K4 最小实验·验收·链接", "第4部分-阶段9-数据绑定与MVVM.md §K1", "勾选验收清单 8 条", () => new AcceptanceChecklistDemo()) { Order = 16 });
        SelectedTopic = Topics.Count > 0 ? Topics[0] : null;
    }
}
