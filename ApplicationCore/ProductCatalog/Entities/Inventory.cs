using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace ApplicationCore.ProductCatalog.Entities;

[Table("Inventory")]
public partial class Inventory
{
    [Key]
    public int ProductId { get; set; }

    public int Quantity { get; set; }

    [Column(TypeName = "money")]
    public decimal Price { get; set; }

    [ForeignKey("ProductId")]
    [InverseProperty("Inventory")]
    public virtual Product Product { get; set; } = null!;
}
