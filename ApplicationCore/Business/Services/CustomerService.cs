﻿using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class CustomerService(ICustomerRepository customerRepository, IAddressService addressService, IContactPreferenceService contactPreferenceService, IRoleService roleService, EagerLoadingContext eagerLoading) : ICustomerService
{
    private readonly ICustomerRepository _customerRepository = customerRepository;
    private readonly IAddressService _addressService = addressService;
    private readonly IContactPreferenceService _contactPreferenceService = contactPreferenceService;
    private readonly IRoleService _roleService = roleService;
    private readonly EagerLoadingContext _eagerLoading = eagerLoading;

    public async Task<OperationResult<CustomerRegistrationDto>> CreateCustomerAsync(CustomerRegistrationDto customer)
    {
        using var transaction = _eagerLoading.Database.BeginTransaction();
        try
        {
            var result = await _customerRepository.FindAsync(c => c.Email == customer.Email);

            if (result.IsSuccess && result.Data.Any())
            {
                await transaction.RollbackAsync();
                return OperationResult<CustomerRegistrationDto>.Failure("Epostadressen finns redan i systemet.");
            }
            else
            {
                var addressModel = await _addressService.CreateAddressAsync(customer.Address);
                if (!addressModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CustomerRegistrationDto>.Failure(addressModel.Message);
                }
                customer.Address.Id = addressModel.Data.Id;

                var contactPreferenceModel = await _contactPreferenceService.CreateContactPreferenceAsync(customer.ContactPreference);
                if (!contactPreferenceModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CustomerRegistrationDto>.Failure(contactPreferenceModel.Message);
                }
                customer.ContactPreference.Id = contactPreferenceModel.Data.Id;

                var roleModel = await _roleService.CreateRoleAsync(customer.Role);
                if (!roleModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CustomerRegistrationDto>.Failure(roleModel.Message);
                }
                customer.Role.Id = roleModel.Data.Id;

                var normalizedFirstName = TextNormalizationHelper.NormalizeText(customer.FirstName).Data;
                var normalizedLastName = TextNormalizationHelper.NormalizeText(customer.LastName).Data;
                var formattedPhoneNumber = TextNormalizationHelper.FormatSwedishPhoneNumber(customer.PhoneNumber).Data;
                var passwordHash = BCrypt.Net.BCrypt.HashPassword(customer.Password);



                var createCustomerEntityResult = await _customerRepository.CreateAsync(new CustomerEntity
                {
                    FirstName = normalizedFirstName,
                    LastName = normalizedLastName,
                    Email = customer.Email,
                    Password = passwordHash,
                    PhoneNumber = formattedPhoneNumber,
                    RoleId = customer.Role.Id,
                    AddressId = customer.Address.Id,
                    ContactPreferenceId = customer.ContactPreference.Id,
                });

                if (!createCustomerEntityResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CustomerRegistrationDto>.Failure("Det gick inte att skapa kundentiteten.");
                }

                var customerEntity = createCustomerEntityResult.Data;

                var newCustomerDto = ConvertToRegistrationDto(createCustomerEntityResult.Data);

                await transaction.CommitAsync();
                return OperationResult<CustomerRegistrationDto>.Success("Kunden skapades framgångrikt", newCustomerDto);
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CustomerRegistrationDto>.Failure("Ett internt fel inträffade när kunden hämtades.");
        }
    }

    public async Task<OperationResult<bool>> DeleteCustomerAsync(int customerId)
    {
        try
        {
            var customerToDeleteResult = await GetCustomerByIdAsync(customerId);

            if (!customerToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Kunden kunde inte hittas.");
            }

            var customerToDelete = customerToDeleteResult.Data;
            {
                if (customerToDelete != null)
                {
                    var result = await _customerRepository.DeleteAsync(c => c.Id == customerToDelete.Id);
                    if (result.IsSuccess)
                    {
                        return OperationResult<bool>.Success("Kunden raderades framgångsrikt.", true);
                    }
                    else
                    {
                        return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av kunden.");
                    }
                }
                else
                {
                    return OperationResult<bool>.Failure("Kunden kunde inte hittas.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när kunden skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<CustomerListDto>>> GetAllCustomersAsync()
    {
        try
        {
            var customerEntitiesResult = await _customerRepository.GetAllAsync();

            if (customerEntitiesResult.IsSuccess && customerEntitiesResult.Data != null)
            {
                var customersDto = customerEntitiesResult.Data.Select(ConvertToCustomerListDto).ToList();

                return OperationResult<IEnumerable<CustomerListDto>>.Success("Kunderna hämtades framgångsrikt.", customersDto);              
            }
            else
            {
                return OperationResult<IEnumerable<CustomerListDto>>.Failure("Inga kunder hittades.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CustomerListDto>>.Failure("Ett internt fel inträffade när kunderna skulle hämtas.");
        }
    }

    public async Task<OperationResult<UpdateCustomerDto>> GetCustomerByIdAsync(int customerId)
    {
        try
        {
            var customerResult = await _customerRepository.GetOneAsync(c => c.Id == customerId);
            if (customerResult.IsSuccess && customerResult.Data != null)
            {
                var customer = customerResult.Data;

                var customerDto = ConvertToUpdateCustomerDto(customerResult.Data);

                return OperationResult<UpdateCustomerDto>.Success("Kunden hämtades framgångsrikt.", customerDto);
            }
            else
            {
                return OperationResult<UpdateCustomerDto>.Failure("Kunden kunde inte hittas.");
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<UpdateCustomerDto>.Failure("Ett internt fel inträffade när kunden hämtades.");
        }
    }

    public async Task<OperationResult<CustomerDto>> GetCustomerForReviewByEmailAsync(string email)
    {
        try
        {
            var customerResult = await _customerRepository.GetOneAsync(c => c.Email == email);
            if (customerResult.IsSuccess && customerResult.Data != null)
            {
                var customer = customerResult.Data;
                var customerDto = new CustomerDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    PhoneNumber = customer.PhoneNumber
                };
                return OperationResult<CustomerDto>.Success("Kunden hittades.", customerDto);
            }
            else
            {
                return OperationResult<CustomerDto>.Failure("Ingen kund med angiven e-postadress hittades.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR :: {ex.Message}");
            return OperationResult<CustomerDto>.Failure("Ett internt fel inträffade när kunden skulle hämtas.");
        }
    }

    public async Task<OperationResult<UpdateCustomerDto>> UpdateCustomerAsync(UpdateCustomerDto updateCustomerDto)
    {
        try
        {
            var getCustomerResult = await _customerRepository.GetOneAsync(c => c.Id == updateCustomerDto.Id);
            if (!getCustomerResult.IsSuccess)
            {
                return OperationResult<UpdateCustomerDto>.Failure("Kunden kunde inte hittas.");
            }

            var entityToUpdate = getCustomerResult.Data;

            if (entityToUpdate != null)
            {
                var addressResult = await _addressService.CreateAddressAsync(updateCustomerDto.Address);
                if (!addressResult.IsSuccess)
                {
                    return OperationResult<UpdateCustomerDto>.Failure("Adressen kunde inte Uppdateras.");
                }

                var preferenceResult = await _contactPreferenceService.CreateContactPreferenceAsync(updateCustomerDto.ContactPreference);
                if (!preferenceResult.IsSuccess)
                {
                    return OperationResult<UpdateCustomerDto>.Failure("Kontaktpreferensen kunde inte Uppdateras.");
                }

                var roleResult = await _roleService.CreateRoleAsync(updateCustomerDto.Role);
                if (!roleResult.IsSuccess)
                {
                    return OperationResult<UpdateCustomerDto>.Failure("Rollen kunde inte Uppdateras.");
                }


                if (entityToUpdate != null)
                {
                    entityToUpdate.FirstName = updateCustomerDto.FirstName;
                    entityToUpdate.LastName = updateCustomerDto.LastName;
                    entityToUpdate.Email = updateCustomerDto.Email;
                    entityToUpdate.PhoneNumber = updateCustomerDto.PhoneNumber;
                    entityToUpdate.RoleId = roleResult.Data.Id;
                    entityToUpdate.AddressId = addressResult.Data.Id;
                    entityToUpdate.ContactPreferenceId = preferenceResult.Data.Id;

                    var updateResult = await _customerRepository.UpdateAsync(
                        c => c.Id == entityToUpdate.Id,
                        entityToUpdate
                    );

                    if (!updateResult.IsSuccess)
                    {
                        return OperationResult<UpdateCustomerDto>.Failure("Det gick inte att uppdatera kunden.");
                    }
                    var updatedEntity = updateResult.Data;
                    var updatedDto = new UpdateCustomerDto
                    {
                        Id = updatedEntity.Id,
                        FirstName = updatedEntity.FirstName,
                        LastName = updatedEntity.LastName,
                        Email = updatedEntity.Email,
                        PhoneNumber = updatedEntity.PhoneNumber,
                        Role = new RoleDto { Id = roleResult.Data.Id, RoleName = updateCustomerDto.Role.RoleName },
                        Address = new AddressDto { Id = addressResult.Data.Id, StreetName = updateCustomerDto.Address.StreetName, PostalCode = updateCustomerDto.Address.PostalCode, City = updateCustomerDto.Address.City },
                        ContactPreference = new ContactPreferenceDto { Id = preferenceResult.Data.Id, PreferredContactMethod = updateCustomerDto.ContactPreference.PreferredContactMethod }
                    };
                    return OperationResult<UpdateCustomerDto>.Success("Kunden uppdaterades framgångsrikt.", updatedDto);

                }
                else
                {
                    return OperationResult<UpdateCustomerDto>.Failure("Kunden kunde inte hittas.");
                }

            }
            else
            {
                return OperationResult<UpdateCustomerDto>.Failure("Kunden kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<UpdateCustomerDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }

    private CustomerRegistrationDto ConvertToRegistrationDto(CustomerEntity customer)
    {
        return new CustomerRegistrationDto
        {
            Id = customer.Id,
            FirstName = customer.FirstName,
            LastName = customer.LastName,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
        };
    }

    private CustomerListDto ConvertToCustomerListDto(CustomerEntity customerEntity)
    {
        return new CustomerListDto
        {
            Id = customerEntity.Id,
            FirstName = customerEntity.FirstName,
            LastName = customerEntity.LastName,
            Email = customerEntity.Email,
            PhoneNumber = customerEntity.PhoneNumber,
            Address = new AddressDto
            {
                Id = customerEntity.Address.Id,
                StreetName = customerEntity.Address.StreetName,
                City = customerEntity.Address.City,
                PostalCode = customerEntity.Address.PostalCode
            },
            Role = new RoleDto
            {
                Id = customerEntity.Role.Id,
                RoleName = customerEntity.Role.RoleName
            },
            ContactPreference = new ContactPreferenceDto
            {
                Id = customerEntity.ContactPreference.Id,
                PreferredContactMethod = customerEntity.ContactPreference.PreferredContactMethod
            }
        };
    }

    private UpdateCustomerDto ConvertToUpdateCustomerDto(CustomerEntity customerEntity)
    {
        return new UpdateCustomerDto
        {
            Id = customerEntity.Id,
            FirstName = customerEntity.FirstName,
            LastName = customerEntity.LastName,
            Email = customerEntity.Email,
            PhoneNumber = customerEntity.PhoneNumber,
            Address = new AddressDto
            {
                Id = customerEntity.Address.Id,
                StreetName = customerEntity.Address.StreetName,
                PostalCode = customerEntity.Address.PostalCode,
                City = customerEntity.Address.City
            },
            Role = new RoleDto
            {
                Id = customerEntity.Role.Id,
                RoleName = customerEntity.Role.RoleName
            },
            ContactPreference = new ContactPreferenceDto
            {
                Id = customerEntity.ContactPreference.Id,
                PreferredContactMethod = customerEntity.ContactPreference.PreferredContactMethod
            }
        };
    }
}
