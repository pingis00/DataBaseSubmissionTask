using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ProductCatalog.Repositories;

public class CategoryRepository(DataContext context) : BaseProductRepository<Category>(context), ICategoryRepository
{
    private readonly DataContext _context = context;

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(c => c.CategoryId == categoryId);
    }
}
