using Part03_03_ProjectStructure.Contracts;
namespace Part03_03_ProjectStructure.Application;
public interface IProductService { IReadOnlyList<ProductDto> List(); }
public interface IProductRepository { IReadOnlyList<Domain.Product> GetAll(); }
public sealed class ProductService(IProductRepository repo) : IProductService {
  public IReadOnlyList<ProductDto> List()=>repo.GetAll().Select(p=>new ProductDto(p.Id,p.Sku,p.Name,p.Price)).ToList();
}
