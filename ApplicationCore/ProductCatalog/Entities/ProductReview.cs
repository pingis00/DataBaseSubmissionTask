using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ProductCatalog.Entities;

[Table("ProductReview")]
public partial class ProductReview
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string? ReviewName { get; set; }

    [StringLength(500)]
    public string? ReviewText { get; set; }

    public int ArticleNumber { get; set; }

    [ForeignKey("ArticleNumber")]
    [InverseProperty("ProductReviews")]
    public virtual Product ArticleNumberNavigation { get; set; } = null!;
}
