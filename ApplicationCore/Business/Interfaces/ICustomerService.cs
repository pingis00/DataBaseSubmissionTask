using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;

namespace ApplicationCore.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<OperationResult<CustomerRegistrationDto>> CreateCustomerAsync(CustomerRegistrationDto customer);
        Task<OperationResult<CustomerRegistrationDto>> GetCustomerByIdAsync(int customerId);
        Task<OperationResult<CustomerRegistrationDto>> GetCustomerByEmailAsync(string email);
        Task<OperationResult<IEnumerable<CustomerListDto>>> GetAllCustomersAsync();
        Task<OperationResult<CustomerRegistrationDto>> UpdateCustomerAsync(CustomerRegistrationDto customerDto);
        Task<OperationResult<bool>> DeleteCustomerAsync(int customerId);
    }
}
