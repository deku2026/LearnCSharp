using Part03_03_ProjectStructure.Application;
using Part03_03_ProjectStructure.Domain;
namespace Part03_03_ProjectStructure.Infrastructure;
public sealed class InMemoryProductRepository : IProductRepository {
  private readonly List<Product> _items=[ new(){Id=Guid.NewGuid(),Sku="CUP-001",Name="马克杯",Price=29.9m} ];
  public IReadOnlyList<Product> GetAll()=>_items;
}
