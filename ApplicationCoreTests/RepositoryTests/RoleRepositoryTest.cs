using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class RoleRepositoryTest
{
    private readonly EagerLoadingContext _context =
        new(new DbContextOptionsBuilder<EagerLoadingContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnRoleEntityWithId_1()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();

        var result = await roleRepository.CreateAsync(roleEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Testroll", result.Data.RoleName);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var roleRepository = new RoleRepository(_context);
        var roleEntity = new RoleEntity();

        var result = await roleRepository.CreateAsync(roleEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();
        var createResult = await roleRepository.CreateAsync(roleEntity);

        var deleteResult = await roleRepository.DeleteAsync(r => r.Id == createResult.Data.Id);
        var findResult = await roleRepository.GetOneAsync(a => a.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();

        var deleteResult = await roleRepository.DeleteAsync(r => r.Id == roleEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnRole()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();
        await roleRepository.CreateAsync(roleEntity);

        var result = await roleRepository.FindAsync(r => r.RoleName == "Testroll");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();
        await roleRepository.CreateAsync(roleEntity);

        var result = await roleRepository.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<RoleEntity>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();
        await roleRepository.CreateAsync(roleEntity);

        var result = await roleRepository.GetOneAsync(r => r.Id == roleEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(roleEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();

        var result = await roleRepository.GetOneAsync(r => r.Id == roleEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();
        var createdEntity = await roleRepository.CreateAsync(roleEntity);
        var updatedEntity = new RoleEntity { Id = createdEntity.Data.Id, RoleName = "bytroll"};
        var findResult = await roleRepository.GetOneAsync(a => a.Id == createdEntity.Data.Id);

        var updateResult = await roleRepository.UpdateAsync(a => a.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(roleEntity.Id, updateResult.Data.Id);
        Assert.Equal("bytroll", updateResult.Data.RoleName);
    }

    [Fact]
    public async Task HasCustomersAsync_WithCustomers_ReturnsTrue()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();
        var customerRepository = new CustomerRepository(_context);

        var savedRole = await roleRepository.CreateAsync(roleEntity);

        var customer = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!",
            RoleId = savedRole.Data.Id
        };
        await customerRepository.CreateAsync(customer);
        var hasCustomers = await roleRepository.HasCustomersAsync(savedRole.Data.Id);

        Assert.True(hasCustomers);
    }

    [Fact]
    public async Task HasCustomersAsync_WithOutCustomers_ReturnsFalse()
    {
        var (roleRepository, roleEntity) = SetupRoleTest();

        var savedRole = await roleRepository.CreateAsync(roleEntity);

        var hasCustomers = await roleRepository.HasCustomersAsync(savedRole.Data.Id);

        Assert.False(hasCustomers);
    }

    private (RoleRepository, RoleEntity) SetupRoleTest()
    {
        var roleRepository = new RoleRepository(_context);
        var roleEntity = new RoleEntity
        {
            RoleName = "Testroll",
        };

        return (roleRepository, roleEntity);
    }
}
