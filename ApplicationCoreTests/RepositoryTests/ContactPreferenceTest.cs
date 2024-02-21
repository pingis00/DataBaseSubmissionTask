using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class ContactPreferenceTest
{
    private readonly EagerLoadingContext _context =
        new(new DbContextOptionsBuilder<EagerLoadingContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnContactPreferenceEntityWithId_1()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();

        var result = await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Brevduva", result.Data.PreferredContactMethod);
    }
    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceEntity = new ContactPreferenceEntity();

        var result = await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();
        var createResult = await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        var deleteResult = await contactPreferenceRepository.DeleteAsync(r => r.Id == createResult.Data.Id);
        var findResult = await contactPreferenceRepository.GetOneAsync(a => a.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();

        var deleteResult = await contactPreferenceRepository.DeleteAsync(r => r.Id == contactPreferenceEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnRole()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();
        await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        var result = await contactPreferenceRepository.FindAsync(r => r.PreferredContactMethod == "Brevduva");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();
        await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        var result = await contactPreferenceRepository.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<ContactPreferenceEntity>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();
        await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        var result = await contactPreferenceRepository.GetOneAsync(c => c.Id == contactPreferenceEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(contactPreferenceEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();

        var result = await contactPreferenceRepository.GetOneAsync(c => c.Id == contactPreferenceEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();
        var createdEntity = await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);
        var updatedEntity = new ContactPreferenceEntity { Id = createdEntity.Data.Id, PreferredContactMethod = "byt kontaktpreferens" };
        var findResult = await contactPreferenceRepository.GetOneAsync(a => a.Id == createdEntity.Data.Id);

        var updateResult = await contactPreferenceRepository.UpdateAsync(a => a.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(contactPreferenceEntity.Id, updateResult.Data.Id);
        Assert.Equal("byt kontaktpreferens", updateResult.Data.PreferredContactMethod);
    }

    [Fact]
    public async Task HasCustomersAsync_WithCustomers_ReturnsTrue()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();
        var customerRepository = new CustomerRepository(_context);

        var savedContactPreference = await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        var customer = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!",
            ContactPreferenceId = savedContactPreference.Data.Id
        };
        await customerRepository.CreateAsync(customer);
        var hasCustomers = await contactPreferenceRepository.HasCustomersAsync(savedContactPreference.Data.Id);

        Assert.True(hasCustomers);
    }

    [Fact]
    public async Task HasCustomersAsync_WithOutCustomers_ReturnsFalse()
    {
        var (contactPreferenceRepository, contactPreferenceEntity) = SetupContactPreferenceTest();

        var savedRole = await contactPreferenceRepository.CreateAsync(contactPreferenceEntity);

        var hasCustomers = await contactPreferenceRepository.HasCustomersAsync(savedRole.Data.Id);

        Assert.False(hasCustomers);
    }

    private (ContactPreferenceRepository, ContactPreferenceEntity) SetupContactPreferenceTest()
    {
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceEntity = new ContactPreferenceEntity
        {
            PreferredContactMethod = "Brevduva",
        };

        return (contactPreferenceRepository, contactPreferenceEntity);
    }
}
