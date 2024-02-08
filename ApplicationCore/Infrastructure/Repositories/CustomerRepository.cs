using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ApplicationCore.Infrastructure.Repositories;

public class CustomerRepository(EagerLoadingContext context) : BaseRepository<CustomerEntity>(context), ICustomerRepository
{
    private readonly EagerLoadingContext _context = context;

    public override async Task<OperationResult<IEnumerable<CustomerEntity>>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Customers
                .Include(x => x.Address)
                .Include(x => x.ContactPreference)
                .Include(x => x.Role)
                .ToListAsync();
            return OperationResult<IEnumerable<CustomerEntity>>.Success("Success", existingEntities);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CustomerEntity>>.Failure(ex.Message);
        }
    }

    public override async Task<OperationResult<CustomerEntity>> GetOneAsync(Expression<Func<CustomerEntity, bool>> predicate)
    {
        try
        {
            var existingEntity = await _context.Customers
                .Include(x => x.Address)
                .Include(x => x.ContactPreference)
                .Include(x => x.Role)
                .FirstOrDefaultAsync(predicate);
            if (existingEntity != null)
            {
                return OperationResult<CustomerEntity>.Success("Entiteten hittades.", existingEntity);
            }
            else
            {
                return OperationResult<CustomerEntity>.Failure("Entiteten hittades inte.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CustomerEntity>.Failure("Ett fel uppstod när entiteten skulle hämtas: " + ex.Message);
        }
    }
}
