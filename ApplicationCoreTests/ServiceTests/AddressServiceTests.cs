using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Services;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.ServiceTests;

public class AddressServiceTests
{
    private readonly EagerLoadingContext _context =
        new(new DbContextOptionsBuilder<EagerLoadingContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task CreateAddressAsyncShould_CreateNewAddress_ReturnTrue()
    {
        var (addressService, addressDto) = SetupAddressTest();

        var result = await addressService.CreateAddressAsync(addressDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Adressen skapades framgångrikt.", result.Message);
    }

    [Fact]
    public async Task CreateAddressAsyncShould_NotCreateAddressIfAlreadyExist_ReturnFalse()
    {
        var (addressService, addressDto) = SetupAddressTest();
        await addressService.CreateAddressAsync(addressDto);

        var result = await addressService.CreateAddressAsync(addressDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Adressen finns redan i systemet.", result.Message);
    }

    [Fact]
    public async Task DeleteAddressAsync_WithNoLinkedCustomers_DeletesAddressSuccessfully()
    {
        var (AddressService, AddressDto) = await SetupAddressTestAsync();
        await AddressService.CreateAddressAsync(AddressDto);

        var result = await AddressService.DeleteAddressAsync(AddressDto.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Adressen raderades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task DeleteAddressAsync_LinkedToCustomers_DeleteFails()
    {
        var (AddressService, AddressDto) = await SetupAddressTestAsync();

        var customerRepository = new CustomerRepository(_context);
        var customerEntity = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!",
            AddressId = AddressDto.Id,
        };

        await customerRepository.CreateAsync(customerEntity);

        var deleteResult = await AddressService.DeleteAddressAsync(AddressDto.Id);


        Assert.False(deleteResult.IsSuccess);
        Assert.Equal("Adressen kan inte raderas eftersom den är kopplad till en eller flera kunder.", deleteResult.Message);
    }

    [Fact]
    public async Task GetAllAddressesAsync_WitAddresses_ReturnsAddresses()
    {
        var (AddressService, _) = await SetupAddressTestAsync();

        var result = await AddressService.GetAllAddressesAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.Equal("Adresserna hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetAllAddressesAsync_NoAddresses_ReturnsFailure()
    {
        var addressRepository = new AddressRepository(_context);
        var addressService = new AddressService(addressRepository);

        var result = await addressService.GetAllAddressesAsync();

        Assert.True(result.IsSuccess, "Inga adresser hittades.");
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetAddressByIdAsync_ReturnsContactPreference()
    {
        var (AddressService, AddressDto) = await SetupAddressTestAsync();

        var result = await AddressService.GetAddressByIdAsync(AddressDto.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(AddressDto.Id, result.Data.Id);
        Assert.Equal("Adressen hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetAddressByIdAsync_AddressDoesNotExist_ReturnsFailure()
    {
        var (AddressService, _) = await SetupAddressTestAsync();
        var nonExistentAddressId = 1999;

        var result = await AddressService.GetAddressByIdAsync(nonExistentAddressId);

        Assert.False(result.IsSuccess);
        Assert.Equal("Adressen kunde inte hittas.", result.Message);
    }

    [Fact]
    public async Task UpdateContactPreferenceAsync_ExistingContactPreference_UpdatesSuccessfully()
    {
        var (addressService, addressDto) = await SetupAddressTestAsync();
        await addressService.CreateAddressAsync(addressDto);

        addressDto.StreetName = "NyGata";
        addressDto.PostalCode = "NyPostkod";
        addressDto.City = "NyStad";
        var updateResult = await addressService.UpdateAddressAsync(addressDto);

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Adressen uppdaterades framgångsrikt.", updateResult.Message);
        Assert.Equal(addressDto.StreetName, updateResult.Data.StreetName);
        Assert.Equal(addressDto.PostalCode, updateResult.Data.PostalCode);
        Assert.Equal(addressDto.City, updateResult.Data.City);
    }

    [Fact]
    public async Task UpdateContactPreferenceAsync_ContactPreferenceDoesNotExist_ReturnsFailure()
    {
        var (AddressService, _) = await SetupAddressTestAsync();
        var nonExistentAddressDto = new AddressDto { 
            Id = 1999, StreetName = "NonExistent", PostalCode = "NonExistent", City = "NonExistent" };

        var result = await AddressService.UpdateAddressAsync(nonExistentAddressDto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Adressen kunde inte hittas.", result.Message);
    }

    private (AddressService, AddressDto) SetupAddressTest()
    {
        var addressRepository = new AddressRepository(_context);
        var addressService = new AddressService(addressRepository);

        var streetName = "testgatan";
        var postalCode = "11111";
        var city = "Helsingborg";
        var addressDto = new AddressDto { StreetName = streetName, PostalCode = postalCode, City = city };

        return (addressService, addressDto);
    }

    private async Task<(AddressService, AddressDto)> SetupAddressTestAsync()
    {
        var addressRepository = new AddressRepository(_context);
        var addressService = new AddressService(addressRepository);

        var streetName = "testgatan";
        var postalCode = "11111";
        var city = "Helsingborg";
        var addressDto = new AddressDto { StreetName = streetName, PostalCode = postalCode, City = city };
        var createResult = await addressService.CreateAddressAsync(addressDto);
        if (createResult.IsSuccess && createResult.Data != null)
        {
            addressDto.Id = createResult.Data.Id;
        }

        return (addressService, addressDto);
    }
}
