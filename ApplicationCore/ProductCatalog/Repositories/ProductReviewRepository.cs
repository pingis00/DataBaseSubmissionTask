using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ApplicationCore.ProductCatalog.Repositories;

public class ProductReviewRepository(DataContext context) : BaseProductRepository<ProductReview>(context), IProductReviewRepository
{
    private readonly DataContext _context = context;

    public override async Task<OperationResult<IEnumerable<ProductReview>>> ProductGetAllAsync()
    {
        try
        {
            var existingEntities = await _context.ProductReviews
                .Include(p => p.Product)
                .ToListAsync();
            return OperationResult<IEnumerable<ProductReview>>.Success("Success", existingEntities);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<ProductReview>>.Failure(ex.Message);
        }
    }
}
