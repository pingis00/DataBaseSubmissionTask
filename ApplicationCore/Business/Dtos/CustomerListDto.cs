﻿namespace ApplicationCore.Business.Dtos;

public class CustomerListDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PhoneNumber { get; set; } = null!;
    public string StreetName { get; set; } = null!;
    public string PostalCode { get; set; } = null!;
    public string City { get; set; } = null!;
    public AddressDto Address { get; set; } = new AddressDto();
    public ContactPreferenceDto ContactPreference { get; set; } = new ContactPreferenceDto();
    public RoleDto Role { get; set; } = new RoleDto();
}
