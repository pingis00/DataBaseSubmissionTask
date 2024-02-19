namespace ApplicationCore.ProductCatalog.Dtos;

public class UpdateProductDto
{
    public int Id { get; set; }
    public int ArticleNumber { get; set; }
    public string Title { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;

    public BrandDto Brand { get; set; } = new BrandDto();
    public CategoryDto Category { get; set; } = new CategoryDto();
}
