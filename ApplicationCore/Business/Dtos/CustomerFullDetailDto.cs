namespace ApplicationCore.Business.Dtos;

public class CustomerFullDetailDto : CustomerRegistrationDto
{
    public List<CustomerReviewDto> Reviews { get; set; } = [];
}
