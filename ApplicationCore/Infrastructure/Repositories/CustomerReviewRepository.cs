using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace ApplicationCore.Infrastructure.Repositories;

public class CustomerReviewRepository(EagerLoadingContext context) : BaseRepository<CustomerReviewEntity>(context), ICustomerReviewRepository
{
    private readonly EagerLoadingContext _context = context;

    public override async Task<OperationResult<IEnumerable<CustomerReviewEntity>>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.CustomersReviews
                .Include(r => r.Customer)
                .ToListAsync();
            return OperationResult<IEnumerable<CustomerReviewEntity>>.Success("Success", existingEntities);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CustomerReviewEntity>>.Failure(ex.Message);
        }
    }
}
