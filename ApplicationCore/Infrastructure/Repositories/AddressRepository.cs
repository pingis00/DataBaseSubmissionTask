using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Infrastructure.Repositories;

public class AddressRepository(EagerLoadingContext context) : BaseRepository<AddressEntity>(context), IAddressRepository
{
    private readonly EagerLoadingContext _context = context;

    public async Task<bool> HasCustomersAsync(int addressId)
    {
        return await _context.Customers.AnyAsync(c => c.AddressId == addressId);
    }
}
