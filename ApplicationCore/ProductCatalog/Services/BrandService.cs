using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Diagnostics;

namespace ApplicationCore.ProductCatalog.Services;

public class BrandService(IBrandRepository brandRepository, DataContext dataContext) : IBrandService
{
    private readonly IBrandRepository _brandRepository = brandRepository;
    private readonly DataContext _dataContext = dataContext;

    public async Task<OperationResult<bool>> BrandHasProductsAsync(int brandId)
    {
        bool hasProducts = await _dataContext.Products.AnyAsync(b => b.Id == brandId);
        if (hasProducts)
        {
            return OperationResult<bool>.Failure("Det finns produkter kopplade till varumärket.");
        }
        return OperationResult<bool>.Success("Det finns inga produkter kopplade till varumärket.");
    }

    public async Task<OperationResult<BrandDto>> CreateBrandAsync(BrandDto brand)
    {
        try
        {
            var normalizedBrandName = TextNormalizationHelper.NormalizeText(brand.BrandName).Data;
            var existingBrandResult = await _brandRepository.ProductGetOneAsync(b => b.BrandName == normalizedBrandName);

            if (existingBrandResult.IsSuccess && existingBrandResult.Data != null)
            {
                var brandDto = new BrandDto
                {
                    Id = existingBrandResult.Data.Id,
                    BrandName = existingBrandResult.Data.BrandName

                };

                return OperationResult<BrandDto>.Success("Varumärket finns redan i systemet.", brandDto);
            }
            else
            {
                var newBrandEntityResult = await _brandRepository.ProductCreateAsync(new Brand
                {
                    BrandName = normalizedBrandName
                });

                if (!newBrandEntityResult.IsSuccess)
                {
                    return OperationResult<BrandDto>.Failure("Det gick inte att skapa varumärket.");
                }
                var newBrandEntity = newBrandEntityResult.Data;

                var newBrandDto = new BrandDto
                {
                    Id = newBrandEntity.Id,
                    BrandName = newBrandEntity.BrandName
                };

                return OperationResult<BrandDto>.Success("Varumärket skapades framgångrikt", newBrandDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<BrandDto>.Failure("Ett internt fel inträffade när varumärket skulle skapas.");
        }
    }

    public async Task<OperationResult<bool>> DeleteBrandAsync(int brandId)
    {
        try
        {
            var brandToDeleteResult = await GetBrandsByIdAsync(brandId);

            if (!brandToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Varumärket kunde inte hittas.");
            }

            OperationResult<bool> hasProductsResult = await BrandHasProductsAsync(brandId);
            if (!hasProductsResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Varumärket kan inte raderas eftersom den är kopplad till en eller flera produkter.");
            }
            var brandToDelete = brandToDeleteResult.Data;


            var result = await _brandRepository.ProductDeleteAsync(b => b.Id == brandToDelete.Id);
            if (result.IsSuccess)
            {
                return OperationResult<bool>.Success("Varumärket raderades framgångsrikt.", true);
            }
            else
            {
                return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av Varumärket.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när Varumärket skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<BrandDto>>> GetAllBrandsAsync()
    {
        try
        {
            var brandEntitiesResult = await _brandRepository.ProductGetAllAsync();

            if (brandEntitiesResult.IsSuccess && brandEntitiesResult.Data != null)
            {
                var brandDto = brandEntitiesResult.Data.Select(brandEntity => new BrandDto
                {
                    Id = brandEntity.Id,
                    BrandName = brandEntity.BrandName,
                }).ToList();

                if (brandDto.Any())
                {
                    return OperationResult<IEnumerable<BrandDto>>.Success("Varumärkena hämtades framgångsrikt.", brandDto);
                }
                else
                {
                    return OperationResult<IEnumerable<BrandDto>>.Failure("Inga varumärken hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<BrandDto>>.Failure("Det gick inte att hämta Varumärkena.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<BrandDto>>.Failure("Ett internt fel inträffade när Varumärkena skulle hämtas.");
        }
    }

    public async Task<OperationResult<BrandDto>> GetBrandsByIdAsync(int brandId)
    {
        try
        {
            var brandResult = await _brandRepository.ProductGetOneAsync(b => b.Id == brandId);
            if (brandResult.IsSuccess && brandResult.Data != null)
            {
                var brand = brandResult.Data;
                var brandDto = new BrandDto
                {
                    Id = brand.Id,
                    BrandName = brand.BrandName
                };
                return OperationResult<BrandDto>.Success("Varumärket hämtades framgångsrikt.", brandDto);
            }
            else
            {
                return OperationResult<BrandDto>.Failure("Varumärket kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<BrandDto>.Failure("Ett internt fel inträffade när rollen hämtades.");
        }
    }

    public async Task<OperationResult<BrandDto>> UpdateBrandAsync(BrandDto brandDto)
    {
        try
        {
            var getBrandResult = await _brandRepository.ProductGetOneAsync(b => b.Id == brandDto.Id);

            if (!getBrandResult.IsSuccess)
            {
                return OperationResult<BrandDto>.Failure("Varumärket kunde inte hittas.");
            }

            var entityToUpdate = getBrandResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.BrandName = brandDto.BrandName;

                var updateResult = await _brandRepository.ProductUpdateAsync(
                    b => b.Id == entityToUpdate.Id,
                    entityToUpdate
                );

                if (updateResult.IsSuccess)
                {
                    var updatedEntity = updateResult.Data;
                    var updatedDto = new BrandDto
                    {
                        Id = updatedEntity.Id,
                        BrandName = updatedEntity.BrandName,
                    };

                    return OperationResult<BrandDto>.Success("Varumärket uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<BrandDto>.Failure("Det gick inte att uppdatera Varumärket.");
                }
            }
            else
            {
                return OperationResult<BrandDto>.Failure("Varumärket kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<BrandDto>.Failure("Ett internt fel inträffade när Varumärket skulle uppdateras.");
        }
    }
}
