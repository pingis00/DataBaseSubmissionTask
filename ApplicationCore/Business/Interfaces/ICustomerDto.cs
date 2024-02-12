using ApplicationCore.Business.Dtos;

namespace ApplicationCore.Business.Interfaces;

public interface ICustomerDto
{
    int Id { get; set; }
    string FirstName { get; set; }
    string LastName { get; set; }
    string Email { get; set; }
    string? Password { get; set; }
    string PhoneNumber { get; set; }
    string StreetName { get; set; }
    string PostalCode { get; set; }
    string City { get; set; }
    string PreferredContactMethod { get; set; }
    string RoleName { get; set; }
    AddressDto Address { get; set; }

    ContactPreferenceDto ContactPreference { get; set; }

    RoleDto Role { get; set; }
}
