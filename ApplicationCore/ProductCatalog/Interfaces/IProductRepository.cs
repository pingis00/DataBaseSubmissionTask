using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.ProductCatalog.Entities;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface IProductRepository : IDataRepository<Product>
{
}
