namespace ApplicationCore.ProductCatalog.Dtos;

public class CompleteProductDto
{
    public int ArticleNumber { get; set; }
    public string Title { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
    public string BrandName { get; set; } = null!;
    public string CategoryName { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal Price { get; set; }

    public BrandDto Brand { get; set; } = new BrandDto();
    public CategoryDto Category { get; set; } = new CategoryDto();
    public InventoryDto Inventory { get; set; } = new InventoryDto();
    
}
