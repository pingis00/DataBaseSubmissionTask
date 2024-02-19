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
    public string ReviewName { get; set; } = null!;

    [StringLength(500)]
    public string ReviewText { get; set; } = null!;

    public int ProductId { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("ProductReviews")]
    public virtual Product Product { get; set; } = null!;
}
