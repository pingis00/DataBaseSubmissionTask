using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;

namespace ApplicationCore.Infrastructure.Repositories;

public class CustomerReviewRepository(EagerLoadingContext context) : BaseRepository<CustomerReviewEntity>(context), ICustomerReviewRepository
{
}
