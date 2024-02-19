namespace ApplicationCore.ProductCatalog.Dtos;

public class ProductReviewDto
{
    public int Id { get; set; }
    public string ReviewName { get; set; } = null!;
    public string ReviewText { get; set; } = null!;
    public int ProductId { get; set; }
    public ProductDto Product { get; set; } = new ProductDto();
}
