using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class CustomerReviewRepositoryTests
{
    private readonly EagerLoadingContext _context =
    new(new DbContextOptionsBuilder<EagerLoadingContext>()
    .UseInMemoryDatabase($"{Guid.NewGuid()}")
    .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnCustomerReviewEntityWithId_1()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();

        var result = await customerReviewRepository.CreateAsync(customerReviewEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("En kommentar", result.Data.Comment);
        Assert.True((DateTime.Now - result.Data.Date).TotalMinutes < 1);
    }
    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var customerReviewRepository = new CustomerReviewRepository(_context);
        var customerReviewEntity = new CustomerReviewEntity();

        var result = await customerReviewRepository.CreateAsync(customerReviewEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();
        var createResult = await customerReviewRepository.CreateAsync(customerReviewEntity);

        var deleteResult = await customerReviewRepository.DeleteAsync(r => r.Id == createResult.Data.Id);
        var findResult = await customerReviewRepository.GetOneAsync(a => a.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();

        var deleteResult = await customerReviewRepository.DeleteAsync(c => c.Id == customerReviewEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnCustomerReview()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();
        await customerReviewRepository.CreateAsync(customerReviewEntity);

        var result = await customerReviewRepository.FindAsync(c => c.Comment == "En kommentar" );

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();
        await customerReviewRepository.CreateAsync(customerReviewEntity);

        var result = await customerReviewRepository.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<CustomerReviewEntity>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();
        await customerReviewRepository.CreateAsync(customerReviewEntity);

        var result = await customerReviewRepository.GetOneAsync(c => c.Id == customerReviewEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(customerReviewEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();

        var result = await customerReviewRepository.GetOneAsync(c => c.Id == customerReviewEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (customerReviewRepository, customerReviewEntity) = await SetupCustomerReviewTest();
        var createdEntity = await customerReviewRepository.CreateAsync(customerReviewEntity);
        var updatedEntity = new CustomerReviewEntity
        {
            Id = createdEntity.Data.Id, Comment = "Ny kommentar", Date = customerReviewEntity.Date, CustomerId = customerReviewEntity.CustomerId
        };
        var findResult = await customerReviewRepository.GetOneAsync(c => c.Id == createdEntity.Data.Id);

        var updateResult = await customerReviewRepository.UpdateAsync(a => a.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(customerReviewEntity.Id, updateResult.Data.Id);
        Assert.Equal("Ny kommentar", updateResult.Data.Comment);
    }

    private async Task<(CustomerReviewRepository, CustomerReviewEntity)> SetupCustomerReviewTest()
    {
        var customerRepository = new CustomerRepository(_context);
        var customerEntity = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!"
        };

        var customerResult = await customerRepository.CreateAsync(customerEntity);
        var createdCustomer = customerResult.Data;

        var customerReviewRepository = new CustomerReviewRepository(_context);
        var customerReviewEntity = new CustomerReviewEntity
        {
            Comment = "En kommentar",
            Date = DateTime.Now,
            CustomerId = createdCustomer.Id,
        };

        return (customerReviewRepository, customerReviewEntity);
    }

}
