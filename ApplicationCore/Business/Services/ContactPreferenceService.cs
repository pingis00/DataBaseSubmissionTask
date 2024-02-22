using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using System.Data;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class ContactPreferenceService(IContactPreferenceRepository contactPreferenceRepository) : IContactPreferenceService
{
    private readonly IContactPreferenceRepository _contactPreferenceRepository = contactPreferenceRepository;

    public async Task<OperationResult<bool>> ContactPreferenceHasCustomersAsync(int contactPreferenceId)
    {
        bool hasCustomers = await _contactPreferenceRepository.HasCustomersAsync(contactPreferenceId);
        if (hasCustomers)
        {
            return OperationResult<bool>.Failure("Det finns kunder kopplade till kontaktpreferensen.");
        }
        return OperationResult<bool>.Success("Det finns inga kunder kopplade till kontaktpreferensen.");
    }

    public async Task<OperationResult<ContactPreferenceDto>> CreateContactPreferenceAsync(ContactPreferenceDto contactPreference)
    {
        try
        {
            var normalizedcontactPreferenceName = TextNormalizationHelper.NormalizeText(contactPreference.PreferredContactMethod).Data;
            var existingEntity = await _contactPreferenceRepository.GetOneAsync(c => c.PreferredContactMethod == normalizedcontactPreferenceName);

            if (existingEntity.IsSuccess && existingEntity.Data != null)
            {
                var preferenceDto = ConvertToDto(existingEntity.Data);

                return OperationResult<ContactPreferenceDto>.Success("Kontaktpreferensen finns redan i systemet.", preferenceDto);
            }
            else
            {
                var newContactPreferenceEntityResult = await _contactPreferenceRepository.CreateAsync(new ContactPreferenceEntity
                {
                    PreferredContactMethod = normalizedcontactPreferenceName
                });

                if (!newContactPreferenceEntityResult.IsSuccess)
                {
                    return OperationResult<ContactPreferenceDto>.Failure("Det gick inte att skapa kontaktpreferensen.");
                }
                var newPreferenceEntity = newContactPreferenceEntityResult.Data;

                var newPreferenceDto = ConvertToDto(newPreferenceEntity);

                return OperationResult<ContactPreferenceDto>.Success("Kontaktpreferensen skapades framgångrikt", newPreferenceDto);
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
            var preferenceToDeleteResult = await GetContactPreferenceByIdAsync(contactPreferenceId);

            if (!preferenceToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Kontaktpreferensen kunde inte hittas.");
            }

            OperationResult<bool> hasCustomersResult = await ContactPreferenceHasCustomersAsync(contactPreferenceId);
            if (!hasCustomersResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Preferensen kan inte raderas eftersom den är kopplad till en eller flera kunder.");
            }
            var preferenceToDelete = preferenceToDeleteResult.Data;


            var result = await _contactPreferenceRepository.DeleteAsync(a => a.Id == preferenceToDelete.Id);
            if (result.IsSuccess)
            {
                return OperationResult<bool>.Success("Kontaktpreferensen raderades framgångsrikt.", true);
            }
            else
            {
                return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av kontaktpreferensen.");
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
            var preferenceEntitiesResult = await _contactPreferenceRepository.GetAllAsync();

            if (preferenceEntitiesResult.IsSuccess && preferenceEntitiesResult.Data != null)
            {
                var preferenceDto = preferenceEntitiesResult.Data.Select(ConvertToDto).ToList();

                if (preferenceDto.Any())
                {
                    return OperationResult<IEnumerable<ContactPreferenceDto>>.Success("Kontaktpreferenserna hämtades framgångsrikt.", preferenceDto);
                }
                else
                {
                    return OperationResult<IEnumerable<ContactPreferenceDto>>.Failure("Inga kontaktpreferenser hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<ContactPreferenceDto>>.Failure("Det gick inte att hämta kontaktpreferenserna.");
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
            var contactPreferenceResult = await _contactPreferenceRepository.GetOneAsync(p => p.Id == contactPreferenceId);
            if (contactPreferenceResult.IsSuccess && contactPreferenceResult.Data != null)
            {
                var preferenceDto = ConvertToDto(contactPreferenceResult.Data);
                return OperationResult<ContactPreferenceDto>.Success("Kontaktpreferensen hämtades framgångsrikt.", preferenceDto);
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
            var getPreferenceResult = await _contactPreferenceRepository.GetOneAsync(p => p.Id == contactPreferenceDto.Id);

            if (!getPreferenceResult.IsSuccess)
            {
                return OperationResult<ContactPreferenceDto>.Failure("Kontakpreferensen kunde inte hittas.");
            }

            var entityToUpdate = getPreferenceResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.PreferredContactMethod = contactPreferenceDto.PreferredContactMethod;

                var updateResult = await _contactPreferenceRepository.UpdateAsync(
                    p => p.Id == entityToUpdate.Id,
                    entityToUpdate
                );

                if (updateResult.IsSuccess)
                {
                    var updatedDto = ConvertToDto(updateResult.Data);

                    return OperationResult<ContactPreferenceDto>.Success("Kontakpreferensen uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<ContactPreferenceDto>.Failure("Det gick inte att uppdatera kontakpreferensen.");
                }
            }
            else
            {
                return OperationResult<ContactPreferenceDto>.Failure("Kontakpreferensen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<ContactPreferenceDto>.Failure("Ett internt fel inträffade när Kontakpreferensen skulle uppdateras.");
        }
    }

    private ContactPreferenceDto ConvertToDto(ContactPreferenceEntity entity)
    {
        return new ContactPreferenceDto
        {
            Id = entity.Id,
            PreferredContactMethod = entity.PreferredContactMethod
        };
    }
}
