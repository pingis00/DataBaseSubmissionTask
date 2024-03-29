﻿namespace ApplicationCore.ProductCatalog.Dtos;

public class ProductDto
{
    public int Id { get; set; }
    public int ArticleNumber { get; set; }
    public string Title { get; set; } = null!;
    public string ProductDescription { get; set; } = null!;
}
