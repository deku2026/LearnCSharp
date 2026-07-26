namespace LearnBlazor.Blazor_Part04_FrontendStateArchitecture.Services;

/// <summary>
/// 最小状态容器：持有状态 + 通知订阅者。
/// 组件 OnInitialized 订阅 <see cref="Changed"/>，Dispose 时取消，收到事件时 StateHasChanged。
/// </summary>
public sealed class CounterState : IDisposable
{
    private int _count;
    public int Count => _count;

    public event Action? Changed;

    public void Increment()
    {
        _count++;
        Changed?.Invoke();
    }

    public void Reset()
    {
        _count = 0;
        Changed?.Invoke();
    }

    public void Dispose() => Changed = null;
}