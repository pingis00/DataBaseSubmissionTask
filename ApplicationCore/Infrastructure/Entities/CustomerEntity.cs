using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ApplicationCore.Infrastructure.Entities;

public class CustomerEntity
{
    public int Id { get; set; }
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string FirstName { get; set; } = null!;
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string LastName { get; set;} = null!;
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Email { get; set; } = null!;
    [Required]
    [Column(TypeName = "varchar(100)")]
    public string Password { get; set; } = null!;
    [Required]
    [Column(TypeName = "nvarchar(50)")]
    public string PhoneNumber { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(AddressEntity))]
    public int AddressId { get; set; }
    public AddressEntity Address { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(RoleEntity))]
    public int RoleId { get; set; }
    public RoleEntity Role { get; set; } = null!;

    [Required]
    [ForeignKey(nameof(ContactPreferenceEntity))]
    public int ContactPreferenceId { get; set; }
    public ContactPreferenceEntity ContactPreference { get; set; } = null!;

    public ICollection<CustomerReviewEntity> CustomerReviewEntities { get; set; } = new List<CustomerReviewEntity>();
}
