using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace LearnBlazor.Blazor_Part15_PerformanceDiagnostics.Components;

public sealed class ManualComponent : IComponent
{
    private RenderHandle _handle;
    private string _message = string.Empty;

    [Parameter] public string Message { get; set; } = string.Empty;

    public void Attach(RenderHandle renderHandle) => _handle = renderHandle;

    public Task SetParametersAsync(ParameterView parameters)
    {
        _message = parameters.GetValueOrDefault<string>(nameof(Message)) ?? string.Empty;
        _handle.Render(BuildRenderTree);
        return Task.CompletedTask;
    }

    private void BuildRenderTree(RenderTreeBuilder builder)
    {
        builder.OpenElement(0, "p");
        builder.AddContent(1, $"手写组件渲染：{_message}");
        builder.CloseElement();
    }
}