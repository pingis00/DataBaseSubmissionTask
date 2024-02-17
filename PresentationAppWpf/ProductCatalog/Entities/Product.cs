using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PresentationAppWpf.ProductCatalog.Entities;

[Keyless]
public partial class Product
{
    public int ArticleNumber { get; set; }

    [StringLength(100)]
    public string Title { get; set; } = null!;

    [StringLength(500)]
    public string ProductDescription { get; set; } = null!;

    public int BrandId { get; set; }

    public int CategoryId { get; set; }

    [ForeignKey("BrandId")]
    public virtual Brand Brand { get; set; } = null!;

    [ForeignKey("CategoryId")]
    public virtual Category Category { get; set; } = null!;
}
