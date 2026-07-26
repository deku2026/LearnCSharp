namespace LearnBlazor.Blazor_Part04_FrontendStateArchitecture.Services;

public sealed record CartItem(string Id, string Name, decimal Price, int Qty);

public sealed class CartState : IDisposable
{
    private readonly List<CartItem> _items = new();
    public IReadOnlyList<CartItem> Items => _items;
    public decimal Total => _items.Sum(i => i.Price * i.Qty);
    public int Count => _items.Sum(i => i.Qty);

    public event Action? Changed;

    public void Add(CartItem item)
    {
        CartItem? existing = _items.FirstOrDefault(i => i.Id == item.Id);
        if (existing is { } e)
        {
            _items.Remove(e);
            _items.Add(e with { Qty = e.Qty + item.Qty });
        }
        else
        {
            _items.Add(item);
        }
        Changed?.Invoke();
    }

    public void Remove(string id)
    {
        _items.RemoveAll(i => i.Id == id);
        Changed?.Invoke();
    }

    public void Clear()
    {
        _items.Clear();
        Changed?.Invoke();
    }

    public void Dispose() => Changed = null;
}
