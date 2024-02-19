namespace ApplicationCore.ProductCatalog.Dtos;

public class InventoryDto
{
    public int Id { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
    public int ProductId { get; set; }
    public string ProductTitle { get; set; } = null!;
    public string ArticleNumber { get; set; } = null!;

    public ProductDto Product { get; set; } = new ProductDto();
}
