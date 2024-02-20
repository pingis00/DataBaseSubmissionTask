using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.ProductCatalog.Entities;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface ICategoryRepository : IDataRepository<Category>
{
    Task<bool> HasProductsAsync(int categoryId);
}
