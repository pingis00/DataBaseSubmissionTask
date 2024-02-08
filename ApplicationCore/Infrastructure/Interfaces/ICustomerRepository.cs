using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Entities;

namespace ApplicationCore.Infrastructure.Interfaces;

public interface ICustomerRepository : IRepository<CustomerEntity>
{
}
