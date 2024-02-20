using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.Infrastructure.Repositories;

public class ContactPreferenceRepository(EagerLoadingContext context) : BaseRepository<ContactPreferenceEntity>(context), IContactPreferenceRepository
{
    private readonly EagerLoadingContext _context = context;

    public async Task<bool> HasCustomersAsync(int contactPreferenceId)
    {
        return await _context.Customers.AnyAsync(c => c.ContactPreferenceId == contactPreferenceId);
    }
}
