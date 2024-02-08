namespace ApplicationCore.Business.Dtos;

public class CustomerRegistrationDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public string PreferredContactMethod { get; set; } = null!;
    public string RoleName { get; set; } = null!;

    public AddressDto Address { get; set; } = null!;

    public ContactPreferenceDto ContactPreference { get; set; } = null!;

    public RoleDto Role { get; set; } = null!;

    public CustomerDto Customer { get; set; } = null!;
}
