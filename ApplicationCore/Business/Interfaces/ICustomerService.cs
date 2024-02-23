using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;

namespace ApplicationCore.Business.Interfaces
{
    public interface ICustomerService
    {
        Task<OperationResult<CustomerRegistrationDto>> CreateCustomerAsync(CustomerRegistrationDto customer);
        Task<OperationResult<UpdateCustomerDto>> GetCustomerByIdAsync(int customerId);
        Task<OperationResult<IEnumerable<CustomerListDto>>> GetAllCustomersAsync();
        Task<OperationResult<UpdateCustomerDto>> UpdateCustomerAsync(UpdateCustomerDto updateCustomerDto);
        Task<OperationResult<bool>> DeleteCustomerAsync(int customerId);
        Task<OperationResult<CustomerDto>> GetCustomerForReviewByEmailAsync(string email);
    }
}
