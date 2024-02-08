using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using System.Data;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class ContactPreferenceService(IContactPreferenceRepository contactPreferenceRepository) : IContactPreferenceService
{
    private readonly IContactPreferenceRepository _contactPreferenceRepository = contactPreferenceRepository;

    public async Task<OperationResult<ContactPreferenceDto>> CreateContactPreferenceAsync(ContactPreferenceDto contactPreference)
    {
        try
        {
            var existingcontactPreferenceResult = await GetContactPreferenceByIdAsync(contactPreference.Id);

            if (existingcontactPreferenceResult.IsSuccess && existingcontactPreferenceResult.Data != null)
            {
                return OperationResult<ContactPreferenceDto>.Success("Adressen finns redan i systemet.", existingcontactPreferenceResult.Data);
            }
            else
            {
                var normalizedContactPreference = TextNormalizationHelper.NormalizeText(contactPreference.PreferredContactMethod).Data;

                var newcontactPreferenceEntity = await _contactPreferenceRepository.CreateAsync(new ContactPreferenceEntity
                {
                    PreferredContactMethod = normalizedContactPreference
                });

                var newContactPreferenceDto = new ContactPreferenceDto
                {
                    Id = newcontactPreferenceEntity.Id,
                    PreferredContactMethod = newcontactPreferenceEntity.PreferredContactMethod
                };

                return OperationResult<ContactPreferenceDto>.Success("Kontaktpreferensen skapades framgångrikt", newContactPreferenceDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<ContactPreferenceDto>.Failure("Ett internt fel inträffade när Kontaktpreferensen skulle skapas.");
        }
    }

    public async Task<OperationResult<bool>> DeleteContactPreferenceAsync(int contactPreferenceId)
    {
        try
        {
            var contactPreferenceIToDelete = GetContactPreferenceByIdAsync(contactPreferenceId);
            if (contactPreferenceIToDelete != null)
            {
                var result = await _contactPreferenceRepository.DeleteAsync(c => c.Id == contactPreferenceIToDelete.Id);
                if (result)
                {
                    return OperationResult<bool>.Success("Kontaktpreferensen raderades framgångsrikt.", true);
                }
                else
                {
                    return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av kontaktpreferensen.");
                }
            }
            else
            {
                return OperationResult<bool>.Failure("Kontaktpreferensen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när kontaktpreferensen skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<ContactPreferenceDto>>> GetAllContactPreferencesAsync()
    {
        try
        {
            var contactEntities = await _contactPreferenceRepository.GetAllAsync();

            var contactPreferences = new List<ContactPreferenceDto>();

            foreach ( var contactPreference in contactEntities)
            {
                contactPreferences.Add(new ContactPreferenceDto
                {
                    Id = contactPreference.Id,
                    PreferredContactMethod = contactPreference.PreferredContactMethod,
                });
            }

            if (contactPreferences.Any())
            {
                return OperationResult<IEnumerable<ContactPreferenceDto>>.Success("Kontaktpreferenserna hämtades framgångsrikt.", contactPreferences);
            }
            else
            {
                return OperationResult<IEnumerable<ContactPreferenceDto>>.Failure("Inga kontaktpreferenser hittades.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<ContactPreferenceDto>>.Failure("Ett internt fel inträffade när kontaktpreferenserna skulle hämtas.");
        }
    }

    public async Task<OperationResult<ContactPreferenceDto>> GetContactPreferenceByIdAsync(int contactPreferenceId)
    {
        try
        {
            var contactPreference = await _contactPreferenceRepository.GetOneAsync(cp => cp.Id == contactPreferenceId);
            if (contactPreference != null)
            {
                var contactPreferenceDto = new ContactPreferenceDto
                {
                    Id = contactPreference.Id,
                    PreferredContactMethod = contactPreference.PreferredContactMethod,
                };

                return OperationResult<ContactPreferenceDto>.Success("Kontaktpreferens hämtades framgångsrikt.", contactPreferenceDto);
            }
            else
            {
                return OperationResult<ContactPreferenceDto>.Failure("Kontaktpreferensen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<ContactPreferenceDto>.Failure("Ett internt fel inträffade när Kontaktpreferensen hämtades.");
        }
    }

    public async Task<OperationResult<ContactPreferenceDto>> UpdateContactPreferenceAsync(ContactPreferenceDto contactPreferenceDto)
    {
        try
        {
            return await CreateContactPreferenceAsync(contactPreferenceDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<ContactPreferenceDto>.Failure("Ett internt fel inträffade när kontektpreferensen skulle uppdateras.");
        }
    }
}
