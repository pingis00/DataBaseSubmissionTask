using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;

namespace ApplicationCore.Business.Services;

public class CustomerReviewService(ICustomerReviewRepository customerReviewRepository) : ICustomerReviewService
{
    private readonly ICustomerReviewRepository _customerReviewRepository = customerReviewRepository;

    public Task<OperationResult<CustomerReviewDto>> CreateCustomerReviewAsync(CustomerReviewDto customerReview)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<bool>> DeleteCustomerReviewAsync(int customerReviewId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<CustomerReviewDto>>> GetAllCustomerReviewsAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<CustomerReviewDto>> GetCustomerReviewByIdAsync(int customerReviewId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<CustomerReviewDto>> UpdateCustomerReviewAsync(CustomerReviewDto customerReviewDto)
    {
        throw new NotImplementedException();
    }
}
