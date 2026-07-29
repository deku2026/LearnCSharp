using LearnMaui.Shared;

namespace LearnMaui.Maui_Stage06_BindingMVVM;

internal static class StageCatalog
{
    public static LearningStage Create(NotesViewModel viewModel)
    {
        LearningTopic[] topics =
        [
            new(LearningLevel.Beginner, "单向数据流",
                "用户意图进入 Command，VM 调用服务并迁移状态，PropertyChanged/集合变化驱动 View。View 不直接写业务服务，服务不引用 Page。",
                "MvvmDemoView → NotesViewModel → INotesService"),
            new(LearningLevel.Beginner, "BindingContext",
                "BindingContext 沿逻辑树继承，View 构造函数从 DI 接收 VM 并一次赋值。模板切换到 item 上下文，必须重新声明类型。",
                "MvvmDemoView.BindingContext = viewModel"),
            new(LearningLevel.Beginner, "Path、Source 与 Mode",
                "Path 定位属性；Source 可来自 BindingContext、RelativeSource、x:Reference 或静态资源；OneWay 展示、TwoWay 编辑、OneTime 稳定命令。",
                "Query TwoWay; RefreshCommand OneTime; Items OneWay"),
            new(LearningLevel.Beginner, "INotifyPropertyChanged",
                "可观察属性只在值真正变化时发精确属性名；依赖属性也要通知，例如 IsBusy 同时通知 IsNotBusy。",
                "[ObservableProperty] + [NotifyPropertyChangedFor]"),
            new(LearningLevel.Beginner, "ObservableCollection",
                "集合通知增删，但不会自动观察 item 内部属性；UI 绑定集合应在 Dispatcher 线程批量更新，大数据避免逐项抖动。",
                "ObservableCollection<NoteItem> Items"),
            new(LearningLevel.Beginner, "ICommand",
                "Command 把用户意图与 UI 事件解耦；CanExecute 表达是否允许执行，状态变化后调用 NotifyCanExecuteChanged。",
                "SaveSelectedCommand + CanSave"),
            new(LearningLevel.Intermediate, "AsyncRelayCommand",
                "异步命令观察异常、禁并发、传递 CancellationToken 并生成取消命令；业务方法返回 Task，不扩散 async void。",
                "[RelayCommand(IncludeCancelCommand=true, AllowConcurrentExecutions=false)]"),
            new(LearningLevel.Intermediate, "编译绑定",
                "x:DataType 让 Path 在构建期验证，模板声明 NoteItem 类型。严格模式帮助发现遗漏的反射绑定并改善 trim/AOT。",
                "MvvmDemoView.xaml x:DataType + MauiStrictXamlCompilation"),
            new(LearningLevel.Intermediate, "Converter 与 VM 边界",
                "纯展示变换可用无状态 Converter；业务规则、校验和异步逻辑留在 VM。简单布尔/枚举切换也可用 DataTrigger。",
                "state stays NotesUiState rather than UI Color values"),
            new(LearningLevel.Intermediate, "RelativeSource",
                "模板需要调用父 VM 命令时可按 AncestorType 绑定；不要用脆弱的深层 x:Reference 穿透命名范围。",
                "DataTemplate item context is explicit"),
            new(LearningLevel.Intermediate, "CommunityToolkit 源生成",
                "ObservableProperty 和 RelayCommand 由 partial 类型生成样板；字段命名、partial 方法和属性依赖必须遵循生成器契约。",
                "partial NotesViewModel + generated commands/properties"),
            new(LearningLevel.Intermediate, "ViewModel 生命周期",
                "页面/VM 默认 Transient，页面 Appearing 触发首次加载，Disappearing 取消。应用级缓存服务可 Singleton，但不持有 View。",
                "DI registrations + MainPage lifecycle forwarding"),
            new(LearningLevel.Advanced, "显式 UI 状态机",
                "Idle/Loading/Content/Empty/Error 是互斥状态，避免 IsBusy、HasError、IsEmpty 多个 bool 组合出非法状态。",
                "NotesUiState State transitions in RefreshAsync"),
            new(LearningLevel.Advanced, "校验",
                "同步字段规则可在 VM/DataAnnotations/ObservableValidator 中实现；跨字段和服务端校验保持可测试，并向 View 暴露错误集合。",
                "CanSave is a synchronous validation gate"),
            new(LearningLevel.Advanced, "WeakReferenceMessenger",
                "跨 VM 的低耦合通知使用明确消息类型和接收者生命周期；它不是业务事件总线，也不能替代导航参数或持久状态。",
                "NoteSavedMessage + SaveReceipt registration/disposal"),
            new(LearningLevel.Advanced, "一次性 UI 效果",
                "Toast、焦点和导航属于 effect，不适合作为可重放 State。用可消费事件或注入的对话框/导航接口，避免旋转后重复弹窗。",
                "Save notification is a typed message, not persistent state"),
            new(LearningLevel.Advanced, "导航与对话框抽象",
                "VM 不引用 Shell、Page、Application.Current 或 DisplayAlert；通过 INavigationService/IDialogService 接口发出意图并在边界实现。",
                "NotesViewModel has only INotesService"),
            new(LearningLevel.Expert, "MultiBinding 边界",
                "当前 MultiBinding 不等同完整编译绑定主路径。关键逻辑优先在 VM 暴露派生属性或使用清晰 Converter，并评估 AOT/反射成本。",
                "IsNotBusy is a generated derived property"),
            new(LearningLevel.Expert, "状态迁移可测试",
                "给服务返回 content/empty/error/cancel，断言 State、Items、ErrorMessage 和 IsBusy 的最终不变量，无需启动模拟器。",
                "RefreshAsync has deterministic state transitions"),
            new(LearningLevel.Expert, "反模式审查",
                "禁止 VM new HttpClient/Page、静态当前页、code-behind 写业务、无 x:DataType、同步阻塞、Page.IsBusy、MessagingCenter 与无主 CTS。",
                "rg audit + constructor dependency boundary"),
        ];

        LearningLab[] labs =
        [
            new("执行异步命令", "通过生成的 AsyncRelayCommand 加载集合并观察 Content 状态。",
                async token =>
                {
                    viewModel.Query = "binding";
                    await viewModel.RefreshCommand.ExecuteAsync(null);
                    token.ThrowIfCancellationRequested();
                    return $"State={viewModel.State}, Items={viewModel.Items.Count}, Busy={viewModel.IsBusy}";
                }),
            new("验证 Empty 状态", "搜索不存在的内容，确认状态机进入 Empty 而不是非法 bool 组合。",
                async token =>
                {
                    viewModel.Query = "does-not-exist";
                    await viewModel.RefreshCommand.ExecuteAsync(null);
                    token.ThrowIfCancellationRequested();
                    return $"State={viewModel.State}, Items={viewModel.Items.Count}";
                }),
            new("验证属性依赖通知", "切换 IsBusy 并捕获 IsBusy 与派生 IsNotBusy 的通知。",
                _ =>
                {
                    List<string?> changed = [];
                    viewModel.PropertyChanged += Handler;
                    viewModel.IsBusy = !viewModel.IsBusy;
                    viewModel.PropertyChanged -= Handler;
                    return Task.FromResult(string.Join(" → ", changed));

                    void Handler(object? sender, System.ComponentModel.PropertyChangedEventArgs args)
                    {
                        changed.Add(args.PropertyName);
                    }
                }),
            new("验证弱消息生命周期", "注册接收者、发送保存消息，再 Dispose 解除注册。",
                async token =>
                {
                    viewModel.Query = string.Empty;
                    await viewModel.RefreshCommand.ExecuteAsync(null);
                    token.ThrowIfCancellationRequested();
                    viewModel.Selected = viewModel.Items.FirstOrDefault();
                    using SaveReceipt receipt = new();
                    viewModel.SaveSelectedCommand.Execute(null);
                    return $"接收保存 ID：{receipt.LastSavedId}";
                }),
            new("构造编译绑定 View", "实例化严格编译的 XAML View，确认 DI 注入同一 VM。",
                _ =>
                {
                    MvvmDemoView view = new(viewModel);
                    return Task.FromResult($"BindingContext same={ReferenceEquals(view.BindingContext, viewModel)}");
                }),
        ];

        return new LearningStage(
            6,
            "绑定、状态与 MVVM",
            "第3部分-阶段6-绑定状态MVVM.md",
            topics,
            labs);
    }
}
