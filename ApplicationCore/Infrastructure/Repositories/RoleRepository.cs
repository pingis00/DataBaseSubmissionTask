using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;

namespace ApplicationCore.Infrastructure.Repositories;

public class RoleRepository(EagerLoadingContext context) : BaseRepository<RoleEntity>(context), IRoleRepository
{
}
