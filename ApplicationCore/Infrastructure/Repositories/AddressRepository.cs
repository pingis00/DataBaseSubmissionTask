using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;

namespace ApplicationCore.Infrastructure.Repositories;

public class AddressRepository(EagerLoadingContext context) : BaseRepository<AddressEntity>(context), IAddressRepository
{
    private readonly EagerLoadingContext _context = context;
}
