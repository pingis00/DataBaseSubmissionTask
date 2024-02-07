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

    public override async Task<IEnumerable<CustomerEntity>> GetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Customers
                .Include(x => x.Address)
                .Include(x => x.ContactPreference)
                .Include(x => x.Role)
                .ToListAsync();
            if (existingEntities != null)
            {
                return existingEntities;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return null!;
    }

    public override async Task<CustomerEntity> GetOneAsync(Expression<Func<CustomerEntity, bool>> predicate)
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
                return existingEntity;
            }
        }
        catch (Exception ex) { Debug.WriteLine("ERROR :: " + ex.Message); }
        return null!;
    }
}
