using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace PresentationAppWpf.ProductCatalog.Entities;

[Table("Inventory")]
public partial class Inventory
{
    [Key]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal Price { get; set; }
}
