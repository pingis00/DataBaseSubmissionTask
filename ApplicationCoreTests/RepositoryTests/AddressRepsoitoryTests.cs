using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class AddressRepsoitoryTests
{
    private readonly EagerLoadingContext _context =
        new(new DbContextOptionsBuilder<EagerLoadingContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnAddressEntityWithId_1()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();

        var result = await addressRepository.CreateAsync(addressEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Bluffgatan", result.Data.StreetName);
        Assert.Equal("12345", result.Data.PostalCode);
        Assert.Equal("Helsingborg", result.Data.City);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var addressRepository = new AddressRepository(_context);
        var addressEntity = new AddressEntity();

        var result = await addressRepository.CreateAsync(addressEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();
        var createResult = await addressRepository.CreateAsync(addressEntity);

        var deleteResult = await addressRepository.DeleteAsync(a => a.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        var findResult = await addressRepository.GetOneAsync(a => a.Id == createResult.Data.Id);
        Assert.Null(findResult.Data);

    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();

        var deleteResult = await addressRepository.DeleteAsync(a => a.Id == addressEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnCompleteAddress()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();
        await addressRepository.CreateAsync(addressEntity);

        var result = await addressRepository.FindAsync(a => a.StreetName == "Bluffgatan" && a.PostalCode == "12345" && a.City == "Helsingborg");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();
        await addressRepository.CreateAsync(addressEntity);

        var result = await addressRepository.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<AddressEntity>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();
        await addressRepository.CreateAsync(addressEntity);

        var result = await addressRepository.GetOneAsync(a => a.Id == addressEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(addressEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();

        var result = await addressRepository.GetOneAsync(a => a.Id == addressEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();
        var createdEntity = await addressRepository.CreateAsync(addressEntity);
        var updatedEntity = new AddressEntity { Id = createdEntity.Data.Id, StreetName = "Felgatan", PostalCode = "54321", City = "Göteborg" };
        var findResult = await addressRepository.GetOneAsync(a => a.Id == createdEntity.Data.Id);

        var updateResult = await addressRepository.UpdateAsync(a => a.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(addressEntity.Id, updateResult.Data.Id);
        Assert.Equal("Göteborg", updateResult.Data.City);
        Assert.Equal("Felgatan", updateResult.Data.StreetName);
        Assert.Equal("54321", updateResult.Data.PostalCode);
        Assert.Equal("Göteborg", findResult.Data.City);
        Assert.Equal("Felgatan", findResult.Data.StreetName);
        Assert.Equal("54321", findResult.Data.PostalCode);
    }

    [Fact]
    public async Task HasCustomersAsync_WithCustomers_ReturnsTrue()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();
        var customerRepository = new CustomerRepository(_context);

        var savedAddress = await addressRepository.CreateAsync(addressEntity);

        var customer = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!",
            AddressId = savedAddress.Data.Id
        };
        await customerRepository.CreateAsync(customer);
        var hasCustomers = await addressRepository.HasCustomersAsync(savedAddress.Data.Id);

        Assert.True(hasCustomers);
    }

    [Fact]
    public async Task HasCustomersAsync_WithOutCustomers_ReturnsFalse()
    {
        var (addressRepository, addressEntity) = SetupAddressTest();

        var savedAddress = await addressRepository.CreateAsync(addressEntity);

        var hasCustomers = await addressRepository.HasCustomersAsync(savedAddress.Data.Id);

        Assert.False(hasCustomers);
    }

    public (AddressRepository, AddressEntity) SetupAddressTest()
    {
        var addressRepository = new AddressRepository(_context);
        var addressEntity = new AddressEntity
        {
            StreetName = "Bluffgatan",
            PostalCode = "12345",
            City = "Helsingborg"
        };

        return (addressRepository, addressEntity);
    }
}
