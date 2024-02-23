using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Services;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.ServiceTests;

public class CustomerServiceTests
{
    private readonly EagerLoadingContext _context =
    new(new DbContextOptionsBuilder<EagerLoadingContext>()
    .UseInMemoryDatabase($"{Guid.NewGuid()}")
    .Options);

    [Fact]
    public async Task CreateCustomerAsync_CreatesCustomerSuccessfully()
    {
        var addressRepository = new AddressRepository(_context);
        var addressService = new AddressService(addressRepository);
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceService = new ContactPreferenceService(contactPreferenceRepository);
        var roleRepository = new RoleRepository(_context);
        var roleService = new RoleService(roleRepository);
        var customerRepository = new CustomerRepository(_context);

        var customerService = new CustomerService(customerRepository, addressService, contactPreferenceService, roleService, _context);

        var addressDto = new AddressDto { StreetName = "Example Street", City = "Example City", PostalCode = "12345" };
        var contactPreferenceDto = new ContactPreferenceDto { PreferredContactMethod = "Email" };
        var roleDto = new RoleDto { RoleName = "Customer" };

        var customerRegistrationDto = new CustomerRegistrationDto
        {
            Email = "test@example.com",
            FirstName = "Test",
            LastName = "User",
            PhoneNumber = "1234567890",
            Password = "StrongPassword123",
            Address = addressDto,
            ContactPreference = contactPreferenceDto,
            Role = roleDto
        };



         var result = await customerService.CreateCustomerAsync(customerRegistrationDto);


        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }
}
