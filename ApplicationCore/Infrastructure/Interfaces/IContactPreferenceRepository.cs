using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.ProductCatalog.Interfaces;

namespace ApplicationCore.Infrastructure.Interfaces;

public interface IContactPreferenceRepository : IRepository<ContactPreferenceEntity>
{
    Task<bool> HasCustomersAsync(int contactPreferenceId);
}
