using LearnBlazor.Blazor_Part07_ListsMediaUI.Models;

namespace LearnBlazor.Blazor_Part07_ListsMediaUI.Services;

public sealed class ItemService
{
    private static readonly IReadOnlyList<Item> All = Enumerable.Range(1, 10_000)
        .Select(i => new Item(i, $"Item {i}", $"Sub {i}", i % 3 == 0 ? "mail" : "star"))
        .ToList();

    public ValueTask<IEnumerable<Item>> GetItemsAsync(int start, int count, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        IEnumerable<Item> slice = All.Skip(start).Take(count);
        return ValueTask.FromResult(slice);
    }

    public IReadOnlyList<Item> GetAll() => All;
}