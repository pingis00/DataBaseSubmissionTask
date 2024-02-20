using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Infrastructure.Repositories;

public class RoleRepository(EagerLoadingContext context) : BaseRepository<RoleEntity>(context), IRoleRepository
{
    private readonly EagerLoadingContext _context = context;
    public async Task<bool> HasCustomersAsync(int roleId)
    {
        return await _context.Customers.AnyAsync(c => c.RoleId == roleId);
    }
}
