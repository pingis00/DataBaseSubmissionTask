using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCore.ProductCatalog.Entities;

[Index("BrandName", Name = "UQ__Brands__2206CE9BF7C88B05", IsUnique = true)]
public partial class Brand
{
    [Key]
    public int Id { get; set; }

    [StringLength(50)]
    public string BrandName { get; set; } = null!;

    [InverseProperty("Brand")]
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
