using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class CustomerRepositoryTests
{
    private readonly EagerLoadingContext _context =
    new(new DbContextOptionsBuilder<EagerLoadingContext>()
    .UseInMemoryDatabase($"{Guid.NewGuid()}")
    .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnCustomerEntityWithId_1()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();

        var result = await customerRepository.CreateAsync(customerEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Test", result.Data.FirstName);
        Assert.Equal("Testsson", result.Data.LastName);
        Assert.Equal("test@example.com", result.Data.Email);
        Assert.Equal("0123456789", result.Data.PhoneNumber);
        Assert.Equal("SecurePassword123", result.Data.Password);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var customerRepository = new CustomerRepository(_context);
        var customerEntity = new CustomerEntity();

        var result = await customerRepository.CreateAsync(customerEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();
        var createResult = await customerRepository.CreateAsync(customerEntity);

        var deleteResult = await customerRepository.DeleteAsync(c => c.Id == createResult.Data.Id);
        var findResult = await customerRepository.GetOneAsync(c => c.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();

        var deleteResult = await customerRepository.DeleteAsync(c => c.Id == customerEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnCustomerReview()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();
        await customerRepository.CreateAsync(customerEntity);

        var result = await customerRepository.FindAsync(c => 
        c.FirstName == "Test" &&
        c.LastName == "Testsson" &&
        c.Email == "test@example.com" &&
        c.PhoneNumber == "0123456789"&&
        c.Password == "SecurePassword123");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();
        await customerRepository.CreateAsync(customerEntity);

        var result = await customerRepository.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<CustomerEntity>>(result.Data);
        Assert.Contains(result.Data, x => x.Id == customerEntity.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();
        await customerRepository.CreateAsync(customerEntity);

        var result = await customerRepository.GetOneAsync(c => c.Id == customerEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(customerEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (customerRepository, customerEntity) = SetupCustomerTest();

        var result = await customerRepository.GetOneAsync(c => c.Id == customerEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (customerRepository, customerEntity) =  SetupCustomerTest();
        var createdEntity = await customerRepository.CreateAsync(customerEntity);
        var updatedEntity = new CustomerEntity
        {
            Id = createdEntity.Data.Id,
            FirstName = "NyTest",
            LastName = "NyTestsson",
            Email = "test@example.com",
            PhoneNumber = "0123456788",
        };
        var findResult = await customerRepository.GetOneAsync(c => c.Id == createdEntity.Data.Id);

        var updateResult = await customerRepository.UpdateAsync(a => a.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(customerEntity.Id, updateResult.Data.Id);
        Assert.Equal("NyTest", updateResult.Data.FirstName);
        Assert.Equal("NyTestsson", updateResult.Data.LastName);
        Assert.Equal("test@example.com", updateResult.Data.Email);
        Assert.Equal("0123456788", updateResult.Data.PhoneNumber);
    }

    private (CustomerRepository, CustomerEntity) SetupCustomerTest()
    {
        var customerRepository = new CustomerRepository(_context);
        var customerEntity = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@example.com",
            PhoneNumber = "0123456789",
            Password = "SecurePassword123",
        };

        return (customerRepository, customerEntity);
    }
}
