using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;

namespace ApplicationCore.Business.Interfaces;

public interface ICustomerReviewService
{
    Task<OperationResult<CustomerReviewDto>> CreateCustomerReviewAsync(CustomerReviewDto customerReviewdDto);
    Task<OperationResult<CustomerReviewDto>> GetCustomerReviewByIdAsync(int reviewId);
    Task<OperationResult<IEnumerable<CustomerReviewDto>>> GetAllCustomerReviewsAsync();
    Task<OperationResult<CustomerReviewDto>> UpdateCustomerReviewAsync(CustomerReviewDto customerReviewDto);
    Task<OperationResult<bool>> DeleteCustomerReviewAsync(int customerReviewId);
    Task<OperationResult<IEnumerable<CustomerReviewDto>>> GetReviewsByCustomerIdAsync(int customerId);
}
