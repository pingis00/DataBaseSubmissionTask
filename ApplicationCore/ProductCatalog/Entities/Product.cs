using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ProductCatalog.Entities;

[Index("ArticleNumber", Name = "UQ__Products__3C99114230320116", IsUnique = true)]
public partial class Product
{
    [Key]
    public int Id { get; set; }

    public int ArticleNumber { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string ProductDescription { get; set; } = null!;

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("BrandId")]
    [InverseProperty("Products")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("CategoryId")]
    [InverseProperty("Products")]
    public virtual Category Category { get; set; } = null!;

    [InverseProperty("Product")]
    public virtual Inventory? Inventory { get; set; }

    [InverseProperty("Product")]
    public virtual ICollection<ProductReview> ProductReviews { get; set; } = new List<ProductReview>();
}
