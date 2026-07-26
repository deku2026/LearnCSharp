using System.Collections.Concurrent;
using LearnBlazor.Blazor_Part12_Api.Models;

namespace LearnBlazor.Blazor_Part12_Api.Services;

public sealed class ItemStore
{
    private readonly ConcurrentDictionary<int, Item> _items = new();

    public ItemStore()
    {
        foreach (int i in Enumerable.Range(1, 50))
        {
            _items[i] = new Item(i, $"Task {i}", i % 3 == 0);
        }
    }

    public IReadOnlyList<Item> GetAll() => _items.Values.OrderBy(x => x.Id).ToList();

    public Item? Get(int id) => _items.TryGetValue(id, out Item? item) ? item : null;

    public Item Add(string title)
    {
        int id = _items.Keys.Count == 0 ? 1 : _items.Keys.Max() + 1;
        Item item = new(id, title, false);
        _items[id] = item;
        return item;
    }
}