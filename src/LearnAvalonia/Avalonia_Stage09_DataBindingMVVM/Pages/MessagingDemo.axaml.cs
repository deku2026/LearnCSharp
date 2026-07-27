// Doc:    第4部分-阶段9-数据绑定与MVVM.md §E3
// Title:  Messaging
// Stage:  Stage09
// Accept: 使用 WeakReferenceMessenger 收发消息
// Notes:  弱引用防泄漏；消息不能替代正规导航参数；注销随 Dispose。
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;

namespace LearnAvalonia.Avalonia_Stage09_DataBindingMVVM.Pages;

public sealed record ThemeChangedMessage(string Theme);

public partial class MessagingDemo : Avalonia.Controls.UserControl
{
    public MessagingDemo()
    {
        InitializeComponent();
        DataContext = new MessagingVm();
    }
}

public sealed partial class MessagingVm : ObservableObject
{
    [ObservableProperty]
    private string _lastMessage = "（无消息）";

    public MessagingVm()
    {
        WeakReferenceMessenger.Default.Register<ThemeChangedMessage>(this, (_, m) => LastMessage = $"收到：{m.Theme}");
    }

    [RelayCommand]
    private void Send()
    {
        WeakReferenceMessenger.Default.Send(new ThemeChangedMessage("Dark"));
    }
}
