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

    AddressDto Address { get; set; }

    ContactPreferenceDto ContactPreference { get; set; }

    RoleDto Role { get; set; }
}
