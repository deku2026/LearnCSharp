using System.Collections.Concurrent;

namespace Step05_MinimalApiAndControllers.Models;

public sealed record Product(Guid Id, string Sku, string Name, decimal Price);
public sealed record CreateProductRequest(string Name, decimal Price, string Sku);
public sealed record Student(string StudentNumber, string FullName, string Major);
public sealed record CreateStudentRequest(string StudentNumber, string FullName, string Major);

public sealed class ProductStore
{
    private readonly ConcurrentDictionary<Guid, Product> _items = new();

    public ProductStore()
    {
        Add("校园马克杯", 29.9m, "CUP-001");
        Add("线性代数教材", 45m, "BK-LA-01");
    }

    public IReadOnlyCollection<Product> GetAll() => _items.Values.OrderBy(p => p.Name).ToArray();
    public bool TryGet(Guid id, out Product product) => _items.TryGetValue(id, out product!);

    public Product Add(string name, decimal price, string sku)
    {
        var p = new Product(Guid.NewGuid(), sku, name, price);
        _items[p.Id] = p;
        return p;
    }

    public bool TryUpdate(Guid id, string name, decimal price, string sku, out Product? product)
    {
        if (!_items.ContainsKey(id))
        {
            product = null;
            return false;
        }

        product = new Product(id, sku, name, price);
        _items[id] = product;
        return true;
    }

    public bool Remove(Guid id) => _items.TryRemove(id, out _);
}

public sealed class StudentStore
{
    private readonly ConcurrentDictionary<string, Student> _items = new(StringComparer.Ordinal);

    public StudentStore()
    {
        Upsert(new Student("2024001001", "张三", "计算机"));
        Upsert(new Student("2024001002", "李四", "软件工程"));
    }

    public IEnumerable<Student> All() => _items.Values.OrderBy(s => s.StudentNumber);
    public Student? Get(string number) => _items.GetValueOrDefault(number);
    public Student Upsert(Student s) { _items[s.StudentNumber] = s; return s; }
    public bool Delete(string number) => _items.TryRemove(number, out _);
}
