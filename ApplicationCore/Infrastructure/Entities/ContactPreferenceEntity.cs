using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Infrastructure.Entities;

public class ContactPreferenceEntity
{
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string PreferredContactMethod { get; set; } = null!;

    public virtual ICollection<CustomerEntity> Customers { get; set; } = new List<CustomerEntity>();
}
