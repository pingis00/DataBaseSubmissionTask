using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Infrastructure.Entities;

public class CustomerReviewEntity
{
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(500)")]
    public string Comment { get; set; } = null!;

    public DateTime Date { get; set; } = DateTime.UtcNow;

    [Required]
    [ForeignKey(nameof(CustomerEntity))]
    public int CustomerId { get; set; }
    public CustomerEntity Customer { get; set; } = null!;

}
