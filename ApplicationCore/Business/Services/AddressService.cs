using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using System.Data;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class AddressService(IAddressRepository addressRepository) : IAddressService
{
    private readonly IAddressRepository _addressRepository = addressRepository;

    public async Task<OperationResult<AddressDto>> CreateAddressAsync(AddressDto address)
    {
        try
        {
            var existingAddressResult = await GetAddressByIdAsync(address.Id);

            if (existingAddressResult.IsSuccess && existingAddressResult.Data != null)
            {
                return OperationResult<AddressDto>.Success("Adressen finns redan i systemet.", existingAddressResult.Data);  
            }
            else
            {
                var normalizedStreetName = TextNormalizationHelper.NormalizeText(address.StreetName).Data;
                var normalizedCity = TextNormalizationHelper.NormalizeText(address.City).Data;
                var normalizedPostalCode = TextNormalizationHelper.FormatSwedishPostalCode(address.PostalCode).Data;


                var newAddressEntityResult = await _addressRepository.CreateAsync(new AddressEntity
                {
                    StreetName = normalizedStreetName,
                    PostalCode = normalizedPostalCode,
                    City = normalizedCity
                });

                if (!newAddressEntityResult.IsSuccess)
                {
                    return OperationResult<AddressDto>.Failure("Det gick inte att skapa adressen.");
                }

                var newAddressEntity = newAddressEntityResult.Data;

                var newAddressDto = new AddressDto
                {
                    Id = newAddressEntity.Id,
                    StreetName = newAddressEntity.StreetName,
                    PostalCode = newAddressEntity.PostalCode,
                    City = newAddressEntity.City
                };

                return OperationResult<AddressDto>.Success("Adressen skapades framgångrikt", newAddressDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<AddressDto>.Failure("Ett internt fel inträffade när adressen skulle skapas.");
        }
    }

    public async Task<OperationResult<bool>> DeleteAddressAsync(int addressId)
    {
        try
        {
            var addressToDeleteResult = await GetAddressByIdAsync(addressId);

            if (!addressToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Adressen kunde inte hittas.");
            }
            var addressToDelete = addressToDeleteResult.Data;

            var result = await _addressRepository.DeleteAsync(a => a.Id == addressToDelete.Id);
            if (result.IsSuccess)
            {
                return OperationResult<bool>.Success("Adressen raderades framgångsrikt.", true);
            }
            else
            {
                return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av adressen.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när adressen skulle raderas.");
        }
    }

    public async Task<OperationResult<AddressDto>> GetAddressByIdAsync(int addressId)
    {
        try
        {
            var addressResult = await _addressRepository.GetOneAsync(a => a.Id == addressId);
            if (addressResult.IsSuccess && addressResult.Data != null)
            {
                var address = addressResult.Data;
                var addressDto = new AddressDto
                {
                    Id = address.Id,
                    StreetName = address.StreetName,
                    PostalCode = address.PostalCode,
                    City = address.City
                };

                return OperationResult<AddressDto>.Success("Adressen hämtades framgångsrikt.", addressDto);
            }
            else
            {
                return OperationResult<AddressDto>.Failure("Adressen kunde inte hittas.");
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<AddressDto>.Failure("Ett internt fel inträffade när adressen hämtas.");
        }
    }

    public async Task<OperationResult<IEnumerable<AddressDto>>> GetAllAddressesAsync()
    {
        try
        {
            var addressEntitiesResult = await _addressRepository.GetAllAsync();

            if (addressEntitiesResult.IsSuccess && addressEntitiesResult.Data != null)
            {
                var addressesDto = addressEntitiesResult.Data.Select(addressEntity => new AddressDto
                {
                    Id = addressEntity.Id,
                    StreetName = addressEntity.StreetName,
                    PostalCode = addressEntity.PostalCode,
                    City = addressEntity.City,
                }).ToList();

                if (addressesDto.Any())
                {
                    return OperationResult<IEnumerable<AddressDto>>.Success("Adresser hämtades framgångsrikt.", addressesDto);
                }
                else
                {
                    return OperationResult<IEnumerable<AddressDto>>.Failure("Inga adresser hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<AddressDto>>.Failure("Det gick inte att hämta adresserna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<AddressDto>>.Failure("Ett internt fel inträffade när adresserna skulle hämtas.");
        }
    }

    public async Task<OperationResult<AddressDto>> UpdateAddressAsync(AddressDto addressDto)
    {
        try
        {
            var getAddressResult = await _addressRepository.GetOneAsync(a => a.Id == addressDto.Id);

            if (!getAddressResult.IsSuccess)
            {
                return OperationResult<AddressDto>.Failure("Adressen kunde inte hittas.");
            }

            var entityToUpdate = getAddressResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.StreetName = addressDto.StreetName;
                entityToUpdate.PostalCode = addressDto.PostalCode;
                entityToUpdate.City = addressDto.City;

                var updateResult = await _addressRepository.UpdateAsync(
                    e => e.Id == entityToUpdate.Id,
                    entityToUpdate                  
                );

                if (updateResult.IsSuccess)
                {
                    var updatedEntity = updateResult.Data;
                    var updatedDto = new AddressDto
                    {
                        Id = updatedEntity.Id,
                        StreetName = updatedEntity.StreetName,
                        PostalCode = updatedEntity.PostalCode,
                        City = updatedEntity.City
                    };

                    return OperationResult<AddressDto>.Success("Adressen uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<AddressDto>.Failure("Det gick inte att uppdatera adressen.");
                }
            }
            else
            {
                return OperationResult<AddressDto>.Failure("Adressen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<AddressDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }
}
