using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Services;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.ServiceTests;

public class ContactPreferenceServiceTests
{
    private readonly EagerLoadingContext _context =
        new(new DbContextOptionsBuilder<EagerLoadingContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task CreateContactPreferenceAsyncShould_CreateNewContactPreference_ReturnTrue()
    {
        var (contactPreferenceService, contactPreferenceDto) = SetupContactPreferenceTest();

        var result = await contactPreferenceService.CreateContactPreferenceAsync(contactPreferenceDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Kontaktpreferensen skapades framgångrikt", result.Message);
        Assert.Equal(contactPreferenceDto.PreferredContactMethod, result.Data.PreferredContactMethod);
    }

    [Fact]
    public async Task CreateContactPreferenceAsyncShould_NotCreateContactPreferenceIfAlreadyExist_ReturnFalse()
    {
        var (contactPreferenceService, contactPreferenceDto) = SetupContactPreferenceTest();
        await contactPreferenceService.CreateContactPreferenceAsync(contactPreferenceDto);

        var result = await contactPreferenceService.CreateContactPreferenceAsync(contactPreferenceDto);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.Equal("Kontaktpreferensen finns redan i systemet.", result.Message);
        Assert.Equal(contactPreferenceDto.PreferredContactMethod, result.Data.PreferredContactMethod);
    }

    [Fact]
    public async Task DeleteContactPreferenceAsync_WithNoLinkedCustomers_DeletesContactPreferenceSuccessfully()
    {
        var (contactPreferenceService, contactPreferenceDto) = await SetupContactPreferenceTestAsync();
        await contactPreferenceService.CreateContactPreferenceAsync(contactPreferenceDto);

        var result = await contactPreferenceService.DeleteContactPreferenceAsync(contactPreferenceDto.Id);

        Assert.True(result.IsSuccess);
        Assert.Equal("Kontaktpreferensen raderades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task DeleteContactPreferenceAsync_LinkedToCustomers_DeleteFails()
    {
        var (contactPreferenceService, contactPreferenceDto) = await SetupContactPreferenceTestAsync();

        var customerRepository = new CustomerRepository(_context);
        var customerEntity = new CustomerEntity
        {
            FirstName = "Test",
            LastName = "Testsson",
            Email = "test@test.test",
            PhoneNumber = "0123456789",
            Password = "Andreas11!",
            ContactPreferenceId = contactPreferenceDto.Id,
        };

        await customerRepository.CreateAsync(customerEntity);

        var deleteResult = await contactPreferenceService.DeleteContactPreferenceAsync(contactPreferenceDto.Id);


        Assert.False(deleteResult.IsSuccess);
        Assert.Equal("Preferensen kan inte raderas eftersom den är kopplad till en eller flera kunder.", deleteResult.Message);
    }

    [Fact]
    public async Task GetAllContactPreferencesAsync_WithContactPreferences_ReturnsContactPreferences()
    {
        var (contactPreferenceService, _) = await SetupContactPreferenceTestAsync();

        var result = await contactPreferenceService.GetAllContactPreferencesAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.Equal("Kontaktpreferenserna hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetAllContactPreferencesAsync_NoContactPreferences_ReturnsFailure()
    {
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceService = new ContactPreferenceService(contactPreferenceRepository);

        var result = await contactPreferenceService.GetAllContactPreferencesAsync();

        Assert.False(result.IsSuccess);
        Assert.Equal("Inga kontaktpreferenser hittades.", result.Message);
    }

    [Fact]
    public async Task GetContactPreferencesByIdAsync_ReturnsContactPreference()
    {
        var (contactPreferenceService, contactPreferenceDto) = await SetupContactPreferenceTestAsync();

        var result = await contactPreferenceService.GetContactPreferenceByIdAsync(contactPreferenceDto.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(contactPreferenceDto.PreferredContactMethod, result.Data.PreferredContactMethod);
        Assert.Equal("Kontaktpreferensen hämtades framgångsrikt.", result.Message);
    }

    [Fact]
    public async Task GetContactPreferencesByIdAsync_ContactPreferenceDoesNotExist_ReturnsFailure()
    {
        var (contactPreferenceService, _) = await SetupContactPreferenceTestAsync();
        var nonExistentContactPreferenceId = 1999;

        var result = await contactPreferenceService.GetContactPreferenceByIdAsync(nonExistentContactPreferenceId);

        Assert.False(result.IsSuccess);
        Assert.Equal("Kontaktpreferensen kunde inte hittas.", result.Message);
    }

    [Fact]
    public async Task UpdateContactPreferenceAsync_ExistingContactPreference_UpdatesSuccessfully()
    {
        var (contactPreferenceService, contactPreferenceDto) = await SetupContactPreferenceTestAsync();
        await contactPreferenceService.CreateContactPreferenceAsync(contactPreferenceDto);

        contactPreferenceDto.PreferredContactMethod = "UpdatedPreferredContactMethod";
        var updateResult = await contactPreferenceService.UpdateContactPreferenceAsync(contactPreferenceDto);

        Assert.True(updateResult.IsSuccess);
        Assert.Equal("Kontakpreferensen uppdaterades framgångsrikt.", updateResult.Message);
        Assert.Equal(contactPreferenceDto.PreferredContactMethod, updateResult.Data.PreferredContactMethod);
    }

    [Fact]
    public async Task UpdateContactPreferenceAsync_ContactPreferenceDoesNotExist_ReturnsFailure()
    {
        var (contactPreferenceService, _) = await SetupContactPreferenceTestAsync();
        var nonExistentcontactPreferenceDto = new ContactPreferenceDto { Id = 1999, PreferredContactMethod = "NonExistent" };

        var result = await contactPreferenceService.UpdateContactPreferenceAsync(nonExistentcontactPreferenceDto);

        Assert.False(result.IsSuccess);
        Assert.Equal("Kontakpreferensen kunde inte hittas.", result.Message);
    }

    private (ContactPreferenceService, ContactPreferenceDto) SetupContactPreferenceTest()
    {
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceService = new ContactPreferenceService(contactPreferenceRepository);

        var preferedContactMethod = "Brevduva";
        var contactPreferenceDto = new ContactPreferenceDto { PreferredContactMethod = preferedContactMethod };

        return (contactPreferenceService, contactPreferenceDto);
    }

    private async Task<(ContactPreferenceService, ContactPreferenceDto)> SetupContactPreferenceTestAsync()
    {
        var contactPreferenceRepository = new ContactPreferenceRepository(_context);
        var contactPreferenceService = new ContactPreferenceService(contactPreferenceRepository);

        var preferedContactMethod = "Brevduva";
        var contactPreferenceDto = new ContactPreferenceDto { PreferredContactMethod = preferedContactMethod };
        var createResult = await contactPreferenceService.CreateContactPreferenceAsync(contactPreferenceDto);
        if (createResult.IsSuccess && createResult.Data != null)
        {
            contactPreferenceDto.Id = createResult.Data.Id;
        }

        return (contactPreferenceService, contactPreferenceDto);
    }
}
