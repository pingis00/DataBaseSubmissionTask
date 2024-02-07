using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class AddressService(IAddressRepository addressRepository) : IAddressService
{
    private readonly IAddressRepository _addressRepository = addressRepository;

    public async Task<OperationResult<AddressDto>> CreateAddressAsync(AddressDto address)
    {
        try
        {
            var existingAddress = await _addressRepository.GetOneAsync(a => 
            a.StreetName == address.StreetName && 
            a.PostalCode == address.PostalCode && 
            a.City == address.City);

            if (existingAddress != null)
            {
                return OperationResult<AddressDto>.Success("Adressen finns redan i systemet.", new AddressDto
                { Id = existingAddress.Id, StreetName =existingAddress.StreetName, PostalCode =existingAddress.PostalCode, City = existingAddress.City});
            }
            else
            {
                var newAddressEntity = await _addressRepository.CreateAsync(new AddressEntity
                {
                    StreetName = address.StreetName,
                    PostalCode = address.PostalCode,
                    City = address.City
                });

                var newAddressDto = new AddressDto
                {
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
            var addressToDelete = GetAddressByIdAsync(addressId);
            if (addressToDelete != null)
            {
                var result = await _addressRepository.DeleteAsync(a => a.Id == addressToDelete.Id);
                if (result)
                {
                    return OperationResult<bool>.Success("Adressen raderades framgångsrikt.", true);
                }
                else
                {
                    return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av adressen.");
                }
            }
            else
            {
                return OperationResult<bool>.Failure("Adressen kunde inte hittas.");
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
            var address = await _addressRepository.GetOneAsync(a => a.Id == addressId);
            if (address != null)
            {
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
            var addresses = await _addressRepository.GetAllAsync();
            
            var addressDtos = addresses.Select(a => new AddressDto
            {
                Id = a.Id,
                StreetName = a.StreetName,
                PostalCode = a.PostalCode,
                City = a.City
            });
            if (addressDtos.Any())
            {
                return OperationResult<IEnumerable<AddressDto>>.Success("Adresser hämtades framgångsrikt.", addressDtos);
            }
            else
            {
                return OperationResult<IEnumerable<AddressDto>>.Failure("Inga adresser hittades.");
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
           return await CreateAddressAsync(addressDto);


        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<AddressDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }
}
