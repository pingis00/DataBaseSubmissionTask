using ApplicationCore.Business.Dtos;
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
        var (customerService, customerRegistrationDto) = SetupCustomerTest();

        var result = await customerService.CreateCustomerAsync(customerRegistrationDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
    }

    [Fact]
    public async Task CreateCustomerAsyncShould_NotCreateCustomerIfEmailAlreadyExist_ReturnFalse()
    {
        var (customerService, customerRegistrationDto) = SetupCustomerTest();
        await customerService.CreateCustomerAsync(customerRegistrationDto);

        var result = await customerService.CreateCustomerAsync(customerRegistrationDto);

        Assert.NotNull(result);
        Assert.False(result.IsSuccess);
        Assert.Equal("Epostadressen finns redan i systemet.", result.Message);
    }

    [Fact]
    public async Task DeleteCustomerAsync_DeletesCustomerSuccessfully()
    {
        var (customerService, customerRegistrationDto) = SetupCustomerTest();
        var createResult = await customerService.CreateCustomerAsync(customerRegistrationDto);

        var result = await customerService.DeleteCustomerAsync(createResult.Data.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Kunden raderades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetAllCustomersAsync_ReturnsCustomers()
    {
        var (customerService, customerRegistrationDto) = SetupCustomerTest();
        await customerService.CreateCustomerAsync(customerRegistrationDto);

        var result = await customerService.GetAllCustomersAsync();
        
        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.Equal("Kunderna hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetAllCustomersAsync_NoCustomers_ReturnsFailure()
    {
        var addressRepository = new AddressRepository(_context);
        var addressService = new AddressService(addressRepository);
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceService = new ContactPreferenceService(contactPreferenceRepository);
        var roleRepository = new RoleRepository(_context);
        var roleService = new RoleService(roleRepository);
        var customerRepository = new CustomerRepository(_context);

        var customerService = new CustomerService(customerRepository, addressService, contactPreferenceService, roleService, _context);

        var result = await customerService.GetAllCustomersAsync();

        Assert.True(result.IsSuccess, "Inga kunder hittades.");
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetACustomerByIdAsync_ReturnsCustomer()
    {
        var (customerService, customerRegistrationDto) = SetupCustomerTest();
        var createResult = await customerService.CreateCustomerAsync(customerRegistrationDto);

        var result = await customerService.GetCustomerByIdAsync(createResult.Data.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal("Kunden hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetCustomerByIdAsync_CustomerDoesNotExist_ReturnsFailure()
    {
        var (customerService, _) = SetupCustomerTest();
        var nonExistentCustomerId = 1999;

        var result = await customerService.GetCustomerByIdAsync(nonExistentCustomerId);

        Assert.False(result.IsSuccess);
        Assert.Equal("Kunden kunde inte hittas.", result.Message);
    }

    [Fact]
    public async Task UpdateCustomerAsync_ExistingCustomer_UpdatesSuccessfully()
    {
        var (customerService, customerRegistrationDto) = SetupCustomerTest();
        var createResult = await customerService.CreateCustomerAsync(customerRegistrationDto);

        if (createResult.IsSuccess && createResult.Data != null)
        {
            customerRegistrationDto.Id = createResult.Data.Id;
        }

        var updateCustomerDto = new UpdateCustomerDto
        {
            Id = customerRegistrationDto.Id,
            FirstName = "NyttNamn",
            LastName = "NyttEfternamn",
            Email = "test@example.com",
            PhoneNumber = "0789654230",
            Role = new RoleDto { RoleName = "nyRoll" },
            ContactPreference = new ContactPreferenceDto { PreferredContactMethod = "NyMetod" },
            Address = new AddressDto { StreetName = "nyGata", PostalCode = "99999", City = "nyStad" }
        };

        var updateResult = await customerService.UpdateCustomerAsync(updateCustomerDto);



        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Kunden uppdaterades framgångsrikt.", updateResult.Message);
        Assert.Equal(updateCustomerDto.FirstName, updateResult.Data.FirstName);
        Assert.Equal(updateCustomerDto.LastName, updateResult.Data.LastName);
        Assert.Equal(updateCustomerDto.Email, updateResult.Data.Email);
        Assert.Equal(updateCustomerDto.PhoneNumber, updateResult.Data.PhoneNumber);

    }
    private (CustomerService, CustomerRegistrationDto) SetupCustomerTest()
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

        return (customerService, customerRegistrationDto);
    }
}
