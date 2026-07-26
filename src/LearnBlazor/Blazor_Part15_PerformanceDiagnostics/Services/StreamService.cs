using System.Runtime.CompilerServices;

namespace LearnBlazor.Blazor_Part15_PerformanceDiagnostics.Services;

public sealed class StreamService
{
    public async IAsyncEnumerable<string> GetItemsAsync(
        int count = 20,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        for (int i = 1; i <= count; i++)
        {
            ct.ThrowIfCancellationRequested();
            await Task.Delay(200, ct);
            yield return $"item {i}";
        }
    }
}
