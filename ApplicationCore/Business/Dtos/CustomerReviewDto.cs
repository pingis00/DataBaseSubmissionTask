namespace ApplicationCore.Business.Dtos;

public class CustomerReviewDto
{
    public int Id { get; set; }
    public string Comment { get; set; } = null!;
    public DateTime Date { get; set; } = DateTime.UtcNow;
    public int CustomerId { get; set; }
    public CustomerDto Customer { get; set; } = new CustomerDto();
}