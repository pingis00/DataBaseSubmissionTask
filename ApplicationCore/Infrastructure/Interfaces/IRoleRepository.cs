using ApplicationCore.Infrastructure.Entities;

namespace ApplicationCore.Infrastructure.Interfaces;

public interface IRoleRepository : IRepository<RoleEntity>
{
    Task<bool> HasCustomersAsync(int roleId);
}
