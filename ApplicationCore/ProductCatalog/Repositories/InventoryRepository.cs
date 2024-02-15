using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

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
}
