using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.Infrastructure.Repositories;
using BCrypt.Net;
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
            var result = await GetCustomerByEmailAsync(customer.Email);

            if (result.IsSuccess && result.Data != null)
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

                var contactPreferenceModel = await _contactPreferenceService.CreateContactPreferenceAsync(customer.ContactPreference);
                if (!contactPreferenceModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CustomerRegistrationDto>.Failure(contactPreferenceModel.Message);
                }

                var roleModel = await _roleService.CreateRoleAsync(customer.Role);
                if (!roleModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CustomerRegistrationDto>.Failure(roleModel.Message);
                }

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

                var newCustomerDto = new CustomerRegistrationDto
                {
                    Id = customerEntity.Id,
                    FirstName = customerEntity.FirstName,
                    LastName = customerEntity.LastName,
                    Email = customerEntity.Email,
                    PhoneNumber = customerEntity.PhoneNumber,
                };
                await transaction.CommitAsync();
                return OperationResult<CustomerRegistrationDto>.Success("Adressen skapades framgångrikt", newCustomerDto);
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
                var customersDto = customerEntitiesResult.Data.Select(customerEntity => new CustomerListDto
                {
                    Id = customerEntity.Id,
                    FirstName = customerEntity.FirstName,
                    LastName = customerEntity.LastName,
                    Email = customerEntity.Email,
                    PhoneNumber = customerEntity.PhoneNumber,
                    StreetName = customerEntity.Address.StreetName,
                    City = customerEntity.Address.City,
                    PostalCode = customerEntity.Address.PostalCode
                }).ToList();

                if (customersDto.Any())
                {
                    return OperationResult<IEnumerable<CustomerListDto>>.Success("Adresser hämtades framgångsrikt.", customersDto);
                }
                else
                {
                    return OperationResult<IEnumerable<CustomerListDto>>.Failure("Inga adresser hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<CustomerListDto>>.Failure("Det gick inte att hämta kunderna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CustomerListDto>>.Failure("Ett internt fel inträffade när adresserna skulle hämtas.");
        }
    }

    public async Task<OperationResult<CustomerRegistrationDto>> GetCustomerByEmailAsync(string email)
    {
        try
        {
            var customer = await _customerRepository.GetOneAsync(c => c.Email == email);

            if (customer != null)
            {
                return OperationResult<CustomerRegistrationDto>.Failure("Epostadressen finns redan registrerad.");
            }
            else
            {
                return OperationResult<CustomerRegistrationDto>.Success("Ingen kund med angiven e-postadress finns, fortsätt med registrering.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CustomerRegistrationDto>.Failure("Ett internt fel inträffade när Emailen skulle kollas.");
        }
    }

    public async Task<OperationResult<CustomerRegistrationDto>> GetCustomerByIdAsync(int customerId)
    {
        try
        {
            var customerResult = await _customerRepository.GetOneAsync(c => c.Id == customerId);
            if (customerResult.IsSuccess && customerResult.Data != null)
            {
                var customer = customerResult.Data;

                var customerDto = new CustomerRegistrationDto
                {
                    Id = customer.Id,
                    FirstName = customer.FirstName,
                    LastName = customer.LastName,
                    Email = customer.Email,
                    Password = customer.Password,
                    PhoneNumber = customer.PhoneNumber,
                    StreetName = customer.Address.StreetName,
                    PostalCode = customer.Address.PostalCode,
                    City = customer.Address.City,
                    RoleName = customer.Role.RoleName,
                    PreferredContactMethod = customer.ContactPreference.PreferredContactMethod
                };

                return OperationResult<CustomerRegistrationDto>.Success("Kunden hämtades framgångsrikt.", customerDto);
            }
            else
            {
                return OperationResult<CustomerRegistrationDto>.Failure("Kunden kunde inte hittas.");
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CustomerRegistrationDto>.Failure("Ett internt fel inträffade när kunden hämtades.");
        }
    }

    public Task<OperationResult<CustomerRegistrationDto>> UpdateCustomerAsync(CustomerRegistrationDto customerDto)
    {
        throw new NotImplementedException();
    }
}
