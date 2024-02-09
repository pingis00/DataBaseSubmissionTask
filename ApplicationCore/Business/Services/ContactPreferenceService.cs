﻿using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.Infrastructure.Repositories;
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
                return OperationResult<ContactPreferenceDto>.Success("Kontaktpreferensen finns redan i systemet.", existingcontactPreferenceResult.Data);
            }
            else
            {
                var normalizedcontactPreferenceName = TextNormalizationHelper.NormalizeText(contactPreference.PreferredContactMethod).Data;

                var newContactPreferenceEntityResult = await _contactPreferenceRepository.CreateAsync(new ContactPreferenceEntity
                {
                    PreferredContactMethod = normalizedcontactPreferenceName
                });

                if (!newContactPreferenceEntityResult.IsSuccess)
                {
                    return OperationResult<ContactPreferenceDto>.Failure("Det gick inte att skapa kontaktpreferensen.");
                }
                var newPreferenceEntity = newContactPreferenceEntityResult.Data;

                var newPreferenceDto = new ContactPreferenceDto
                {
                    Id = newPreferenceEntity.Id,
                    PreferredContactMethod = newPreferenceEntity.PreferredContactMethod
                };

                return OperationResult<ContactPreferenceDto>.Success("Kontaktppreferensen skapades framgångrikt", newPreferenceDto);
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
                var preferenceDto = preferenceEntitiesResult.Data.Select(preferenceEntity => new ContactPreferenceDto
                {
                    Id = preferenceEntity.Id,
                    PreferredContactMethod = preferenceEntity.PreferredContactMethod
                }).ToList();

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
                var preference = contactPreferenceResult.Data;
                var preferenceDto = new ContactPreferenceDto
                {
                    Id = preference.Id,
                    PreferredContactMethod = preference.PreferredContactMethod
                };
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
                return OperationResult<ContactPreferenceDto>.Failure("Rollen kunde inte hittas.");
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
                    var updatedEntity = updateResult.Data;
                    var updatedDto = new ContactPreferenceDto
                    {
                        Id = updatedEntity.Id,
                        PreferredContactMethod = updatedEntity.PreferredContactMethod
                    };

                    return OperationResult<ContactPreferenceDto>.Success("Adressen uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<ContactPreferenceDto>.Failure("Det gick inte att uppdatera adressen.");
                }
            }
            else
            {
                return OperationResult<ContactPreferenceDto>.Failure("Adressen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<ContactPreferenceDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }
}