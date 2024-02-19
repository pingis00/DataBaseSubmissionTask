using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ApplicationCore.ProductCatalog.Repositories;

public class InventoryRepository(DataContext context) : BaseProductRepository<Inventory>(context), IIventoryRepository
{
    private readonly DataContext _context = context;

    public override async Task<OperationResult<IEnumerable<Inventory>>> ProductGetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Inventories
                .Include(p => p.Product)
                .ToListAsync();
            return OperationResult<IEnumerable<Inventory>>.Success("Success", existingEntities);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<Inventory>>.Failure(ex.Message);
        }
    }

    public override async Task<OperationResult<Inventory>> ProductGetOneAsync(Expression<Func<Inventory, bool>> predicate)
    {
        try
        {
            var existingEntity = await _context.Inventories
                .Include(p => p.Product)
                .FirstOrDefaultAsync(predicate);
            if (existingEntity != null)
            {
                return OperationResult<Inventory>.Success("Entiteten hittades.", existingEntity);
            }
            else
            {
                return OperationResult<Inventory>.Failure("Entiteten hittades inte.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<Inventory>.Failure("Ett fel uppstod när entiteten skulle hämtas: " + ex.Message);
        }
    }

}
