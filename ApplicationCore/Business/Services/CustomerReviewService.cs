using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;

using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class CustomerReviewService(ICustomerReviewRepository customerReviewRepository, ICustomerService customerService) : ICustomerReviewService
{
    private readonly ICustomerReviewRepository _customerReviewRepository = customerReviewRepository;
    private readonly ICustomerService _customerService = customerService;

    public async Task<OperationResult<CustomerReviewDto>> CreateCustomerReviewAsync(CustomerReviewDto customerReviewDto)
    {
        try
        {
            var customerResult = await _customerService.GetCustomerForReviewByEmailAsync(customerReviewDto.Customer.Email);
            if (!customerResult.IsSuccess)
            {
                return OperationResult<CustomerReviewDto>.Failure("Ingen kund med angiven e-postadress hittades. Du behöver vara registrerad för att lämna en recension.");
            }

            var customer = customerResult.Data;
            var customerReviewEntity = new CustomerReviewEntity
            {
                Comment = customerReviewDto.Comment,
                Date = customerReviewDto.Date,
                CustomerId = customer.Id
            };

            var reviewResult = await _customerReviewRepository.CreateAsync(customerReviewEntity);
            if (!reviewResult.IsSuccess)
            {
                return OperationResult<CustomerReviewDto>.Failure("Det gick inte att skapa recensionen.");
            }

            var createdReviewDto = ConvertToDto(reviewResult.Data);
            return OperationResult<CustomerReviewDto>.Success("Recensionen skapades framgångsrikt.", createdReviewDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR :: {ex.Message}");
            return OperationResult<CustomerReviewDto>.Failure("Ett internt fel inträffade när recensionen skulle skapas.");
        }
    }

    public async Task<OperationResult<bool>> DeleteCustomerReviewAsync(int customerReviewId)
    {
        try
        {
            var reviewToDeleteResult = await GetCustomerReviewByIdAsync(customerReviewId);

            if (!reviewToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Recensionen kunde inte hittas.");
            }
            var reviewToDelete = reviewToDeleteResult.Data;


            var result = await _customerReviewRepository.DeleteAsync(r => r.Id == reviewToDelete.Id);
            if (result.IsSuccess)
            {
                return OperationResult<bool>.Success("Recensionen raderades framgångsrikt.", true);
            }
            else
            {
                return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av Recensionen.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när Recensionen skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<CustomerReviewDto>>> GetAllCustomerReviewsAsync()
    {
        try
        {
            var reviewEntitiesResult = await _customerReviewRepository.GetAllAsync();

            if (reviewEntitiesResult.IsSuccess && reviewEntitiesResult.Data != null)
            {
                var reviewDto = reviewEntitiesResult.Data.Select(reviewEntity => ConvertToDto(reviewEntity, true)).ToList();

                if (reviewDto.Any())
                {
                    return OperationResult<IEnumerable<CustomerReviewDto>>.Success("Recensionerna hämtades framgångsrikt.", reviewDto);
                }
                else
                {
                    return OperationResult<IEnumerable<CustomerReviewDto>>.Failure("Inga Recensioner hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<CustomerReviewDto>>.Failure("Det gick inte att hämta Recensionerna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CustomerReviewDto>>.Failure("Ett internt fel inträffade när adresserna skulle hämtas.");
        }
    }

    public async Task<OperationResult<CustomerReviewDto>> GetCustomerReviewByIdAsync(int reviewId)
    {
        try
        {
            var reviewResult = await _customerReviewRepository.GetOneAsync(r => r.Id == reviewId);
            if (!reviewResult.IsSuccess || reviewResult.Data == null)
            {
                return OperationResult<CustomerReviewDto>.Failure("Recensionen kunde inte hittas.");
            }

            var reviewEntity = reviewResult.Data;

            var reviewDto = ConvertToDto(reviewEntity);
            return OperationResult<CustomerReviewDto>.Success("Recensionen hämtades framgångsrikt.", reviewDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR :: {ex.Message}");
            return OperationResult<CustomerReviewDto>.Failure("Ett internt fel inträffade när recensionen hämtades.");
        }
    }

    public async Task<OperationResult<IEnumerable<CustomerReviewDto>>> GetReviewsByCustomerIdAsync(int customerId)
    {
        try
        {
            var reviewEntitiesResult = await _customerReviewRepository.FindAsync(r => r.CustomerId == customerId);

            if (reviewEntitiesResult.IsSuccess && reviewEntitiesResult.Data != null)
            {
                var reviewDtos = reviewEntitiesResult.Data.Select(reviewEntity => new CustomerReviewDto
                {
                    Id = reviewEntity.Id,
                    Comment = reviewEntity.Comment,
                    Date = reviewEntity.Date,
                    CustomerId = reviewEntity.CustomerId,
                }).ToList();

                return OperationResult<IEnumerable<CustomerReviewDto>>.Success("Recensionerna för kunden hämtades framgångsrikt.", reviewDtos);
            }
            else
            {
                return OperationResult<IEnumerable<CustomerReviewDto>>.Failure("Inga recensioner för kunden hittades.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CustomerReviewDto>>.Failure("Ett internt fel inträffade när recensionerna för kunden skulle hämtas.");
        }
    }

    public async Task<OperationResult<CustomerReviewDto>> UpdateCustomerReviewAsync(CustomerReviewDto customerReviewDto)
    {
        try
        {
            var getReviewResult = await _customerReviewRepository.GetOneAsync(r => r.Id == customerReviewDto.Id);

            if (!getReviewResult.IsSuccess)
            {
                return OperationResult<CustomerReviewDto>.Failure("Recensionen kunde inte hittas.");
            }

            var entityToUpdate = getReviewResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.Comment = customerReviewDto.Comment;

                var updateResult = await _customerReviewRepository.UpdateAsync(
                    r => r.Id == entityToUpdate.Id,
                    entityToUpdate
                );

                if (updateResult.IsSuccess)
                {
                    var updatedDto = ConvertToDto(updateResult.Data);

                    return OperationResult<CustomerReviewDto>.Success("Recensionen uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<CustomerReviewDto>.Failure("Det gick inte att uppdatera recensionen.");
                }
            }
            else
            {
                return OperationResult<CustomerReviewDto>.Failure("Adressen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CustomerReviewDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }

    private CustomerReviewDto ConvertToDto(CustomerReviewEntity reviewEntity, bool previewComment = false)
    {
        const int previewLength = 40;
        string comment = previewComment && reviewEntity.Comment.Length > previewLength
                         ? reviewEntity.Comment.Substring(0, previewLength) + "..."
                         : reviewEntity.Comment;
        return new CustomerReviewDto
        {
            Id = reviewEntity.Id,
            Comment = comment,
            Date = reviewEntity.Date,
            CustomerId = reviewEntity.CustomerId,
            Customer = reviewEntity.Customer != null ? new CustomerDto
            {
                Id = reviewEntity.Customer.Id,
                Email = reviewEntity.Customer.Email,
                FirstName = reviewEntity.Customer.FirstName,
                LastName = reviewEntity.Customer.LastName
            } : null!
        };
    }
}
