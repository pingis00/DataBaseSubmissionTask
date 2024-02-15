using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.ProductCatalog.Interfaces;

namespace ApplicationCore.Infrastructure.Interfaces;

public interface ICustomerRepository : IRepository<CustomerEntity>
{
}
