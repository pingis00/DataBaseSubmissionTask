using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ApplicationCore.ProductCatalog.Repositories;

public class ProductRepository(DataContext context) : BaseProductRepository<Product>(context), IProductRepository
{
    private readonly DataContext _context = context;

    public override async Task<OperationResult<IEnumerable<Product>>> ProductGetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Inventory)
                .ToListAsync();
            return OperationResult<IEnumerable<Product>>.Success("Success", existingEntities);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<Product>>.Failure(ex.Message);
        }
    }

    public override async Task<OperationResult<Product>> ProductGetOneAsync(Expression<Func<Product, bool>> predicate)
    {
        try
        {
            var existingEntity = await _context.Products
                .Include(x => x.Category)
                .Include(x => x.Brand)
                .Include(x => x.Inventory)
                .FirstOrDefaultAsync(predicate);
            if (existingEntity != null)
            {
                return OperationResult<Product>.Success("Entiteten hittades.", existingEntity);
            }
            else
            {
                return OperationResult<Product>.Failure("Entiteten hittades inte.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<Product>.Failure("Ett fel uppstod när entiteten skulle hämtas: " + ex.Message);
        }
    }
}
