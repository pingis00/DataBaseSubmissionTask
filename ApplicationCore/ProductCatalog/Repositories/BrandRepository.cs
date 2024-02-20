using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ProductCatalog.Repositories;

public class BrandRepository(DataContext context) : BaseProductRepository<Brand>(context), IBrandRepository
{
    private readonly DataContext _context = context;

    public async Task<bool> HasProductsAsync(int brandId)
    {
        return await _context.Products.AnyAsync(b => b.BrandId == brandId);
    }
}
