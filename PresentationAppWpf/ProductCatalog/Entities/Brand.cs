using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PresentationAppWpf.ProductCatalog.Entities;

[Index("Brandname", Name = "UQ__Brands__62FFFD1C13AD2A51", IsUnique = true)]
public partial class Brand
{
    [Key]
    public int Id { get; set; }

    [Column("BRANDNAME")]
    [StringLength(50)]
    public string Brandname { get; set; } = null!;
}
