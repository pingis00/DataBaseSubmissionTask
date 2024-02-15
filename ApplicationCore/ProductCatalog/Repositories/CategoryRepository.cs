using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;

namespace ApplicationCore.ProductCatalog.Repositories;

public class CategoryRepository(DataContext context) : BaseProductRepository<Category>(context), ICategoryRepository
{
    private readonly DataContext _context = context;
}
