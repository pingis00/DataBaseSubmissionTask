using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Services;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.ServiceTests;

public class RoleServiceTests
{
    private readonly EagerLoadingContext _context =
    new(new DbContextOptionsBuilder<EagerLoadingContext>()
    .UseInMemoryDatabase($"{Guid.NewGuid()}")
    .Options);

    [Fact]
    public async Task CreateRoleAsyncShould_CreateNewRole_ReturnTrue()
    {
        var (roleService, roleDto) = SetupRoleTest();

        var result = await roleService.CreateRoleAsync(roleDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Rollen skapades framgångrikt", result.Message);
        Assert.Equal(roleDto.RoleName, result.Data.RoleName);
    }

    [Fact]
    public async Task CreateRoleAsyncShould_NotCreateRoleIfAlreadyExist_ReturnFalse()
    {
        var (roleService, roleDto) = SetupRoleTest();
        await roleService.CreateRoleAsync(roleDto);

        var result = await roleService.CreateRoleAsync(roleDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Rollen finns redan i systemet.", result.Message);
        Assert.Equal(roleDto.RoleName, result.Data.RoleName);
    }

    [Fact]
    public async Task DeleteRoleAsync_WithNoLinkedCustomers_DeletesRoleSuccessfully()
    {
        var (roleService, roleDto) = await SetupRoleTestAsync();
        await roleService.CreateRoleAsync(roleDto);

        var result = await roleService.DeleteRoleAsync(roleDto.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Rollen raderades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task DeleteRoleAsync_LinkedToCustomers_DeleteFails()
    {
        var (roleService, roleDto) = await SetupRoleTestAsync();

        var customerRepository = new CustomerRepository(_context);
        var customerEntity = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!",
            RoleId = roleDto.Id,
        };

        await customerRepository.CreateAsync(customerEntity);

        var deleteResult = await roleService.DeleteRoleAsync(roleDto.Id);


        Assert.False(deleteResult.IsSuccess);
        Assert.Equal("Rollen kan inte raderas eftersom den är kopplad till en eller flera kunder.", deleteResult.Message);
    }

    [Fact]
    public async Task GetAllRolesAsync_WithRoles_ReturnsRoles()
    {
        var (roleService, _) = await SetupRoleTestAsync();

        var result = await roleService.GetAllRolesAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.Equal("Rollerna hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetAllRolesAsync_NoRoles_ReturnsFailure()
    {
        var roleRepository = new RoleRepository(_context);
        var roleService = new RoleService(roleRepository);

        var result = await roleService.GetAllRolesAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Inga roller hittades.", result.Message);
    }

    [Fact]
    public async Task GetRolesByIdAsync_ReturnsRole()
    {
        var (roleService, roleDto) = await SetupRoleTestAsync();

        var result = await roleService.GetRolesByIdAsync(roleDto.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(roleDto.RoleName, result.Data.RoleName);
        Assert.Equal("Rollen hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetRolesByIdAsync_RoleDoesNotExist_ReturnsFailure()
    {
        var (roleService, _) = await SetupRoleTestAsync();
        var nonExistentRoleId = 1999;

        var result = await roleService.GetRolesByIdAsync(nonExistentRoleId);

        Assert.False(result.IsSuccess);
        Assert.Equal("Rollen kunde inte hittas.", result.Message);
    }

    [Fact]
    public async Task UpdateRoleAsync_ExistingRole_UpdatesSuccessfully()
    {
        var (roleService, roleDto) = await SetupRoleTestAsync();
        await roleService.CreateRoleAsync(roleDto);

        roleDto.RoleName = "UpdatedRoleName";
        var updateResult = await roleService.UpdateRoleAsync(roleDto);

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Rollen uppdaterades framgångsrikt.", updateResult.Message);
        Assert.Equal(roleDto.RoleName, updateResult.Data.RoleName);
    }

    [Fact]
    public async Task UpdateRoleAsync_RoleDoesNotExist_ReturnsFailure()
    {
        var (roleService, _) = await SetupRoleTestAsync();
        var nonExistentRoleDto = new RoleDto { Id = 1999, RoleName = "NonExistent" };

        var result = await roleService.UpdateRoleAsync(nonExistentRoleDto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Rollen kunde inte hittas.", result.Message);
    }

    private (RoleService, RoleDto) SetupRoleTest()
    {
        var roleRepository = new RoleRepository(_context);
        var roleService = new RoleService(roleRepository);

        var roleName = "Test";
        var roleDto = new RoleDto { RoleName = roleName };

        return (roleService, roleDto);
    }

    private async Task<(RoleService, RoleDto)> SetupRoleTestAsync()
    {
        var roleRepository = new RoleRepository(_context);
        var roleService = new RoleService(roleRepository);

        var roleName = "Test";
        var roleDto = new RoleDto { RoleName = roleName };
        var createResult = await roleService.CreateRoleAsync(roleDto);
        if (createResult.IsSuccess && createResult.Data != null)
        {
            roleDto.Id = createResult.Data.Id;
        }

        return (roleService, roleDto);
    }
}
