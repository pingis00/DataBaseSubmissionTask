using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;

namespace ApplicationCore.Infrastructure.Repositories;

public class ContactPreferenceRepository(EagerLoadingContext context) : BaseRepository<ContactPreferenceEntity>(context), IContactPreferenceRepository
{
    private readonly EagerLoadingContext _context = context;
}
