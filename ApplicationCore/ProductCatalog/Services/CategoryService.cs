using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using System.Diagnostics;


namespace ApplicationCore.ProductCatalog.Services
{
    public class CategoryService(ICategoryRepository categoryRepository) : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository = categoryRepository;

        public async Task<OperationResult<bool>> CategoryHasProductsAsync(int categoryId)
        {
            bool hasProducts = await _categoryRepository.HasProductsAsync(categoryId);
            if (hasProducts)
            {
                return OperationResult<bool>.Failure("Det finns produkter kopplade till kategorin.");
            }
            return OperationResult<bool>.Success("Det finns inga produkter kopplade till kategorin.");
        }

        public async Task<OperationResult<CategoryDto>> CreateCategoryAsync(CategoryDto category)
        {
            try
            {
                var normalizedCategoryName = TextNormalizationHelper.NormalizeText(category.CategoryName).Data;
                var existingCategoryResult = await _categoryRepository.ProductGetOneAsync(c => c.CategoryName == normalizedCategoryName);

                if (existingCategoryResult.IsSuccess && existingCategoryResult.Data != null)
                {
                    var categoryDto = new CategoryDto
                    {
                        Id = existingCategoryResult.Data.Id,
                        CategoryName = existingCategoryResult.Data.CategoryName

                    };

                    return OperationResult<CategoryDto>.Success("kategorin finns redan i systemet.", categoryDto);
                }
                else
                {
                    var newCategoryEntityResult = await _categoryRepository.ProductCreateAsync(new Category
                    {
                        CategoryName = normalizedCategoryName
                    });

                    if (!newCategoryEntityResult.IsSuccess)
                    {
                        return OperationResult<CategoryDto>.Failure("Det gick inte att skapa kategorin.");
                    }
                    var newcategoryDto = ConvertToDto(newCategoryEntityResult.Data);

                    return OperationResult<CategoryDto>.Success("Kategorin skapades framgångrikt", newcategoryDto);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR :: " + ex.Message);
                return OperationResult<CategoryDto>.Failure("Ett internt fel inträffade när kategorin skulle skapas.");
            }
        }

        public async Task<OperationResult<bool>> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                var categoryToDeleteResult = await GetCategoryByIdAsync(categoryId);

                if (!categoryToDeleteResult.IsSuccess)
                {
                    return OperationResult<bool>.Failure("Kategorin kunde inte hittas.");
                }

                OperationResult<bool> hasProductsResult = await CategoryHasProductsAsync(categoryId);
                if (!hasProductsResult.IsSuccess)
                {
                    return OperationResult<bool>.Failure("Kategorin kan inte raderas eftersom den är kopplad till en eller flera produkter.");
                }
                var categoryToDelete = categoryToDeleteResult.Data;


                var result = await _categoryRepository.ProductDeleteAsync(c => c.Id == categoryToDelete.Id);
                if (result.IsSuccess)
                {
                    return OperationResult<bool>.Success("Kategorin raderades framgångsrikt.", true);
                }
                else
                {
                    return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av kategorin.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR :: " + ex.Message);
                return OperationResult<bool>.Failure("Ett internt fel inträffade när kategorin skulle raderas.");
            }
        }

        public async Task<OperationResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync()
        {
            try
            {
                var categoryEntitiesResult = await _categoryRepository.ProductGetAllAsync();

                if (categoryEntitiesResult.IsSuccess && categoryEntitiesResult.Data != null)
                {
                    var categoryDto = categoryEntitiesResult.Data.Select(ConvertToDto).ToList();

                    if (categoryDto.Any())
                    {
                        return OperationResult<IEnumerable<CategoryDto>>.Success("Kategorierna hämtades framgångsrikt.", categoryDto);
                    }
                    else
                    {
                        return OperationResult<IEnumerable<CategoryDto>>.Failure("Inga Kategorier hittades.");
                    }
                }
                else
                {
                    return OperationResult<IEnumerable<CategoryDto>>.Failure("Det gick inte att hämta Kategorierna.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR :: " + ex.Message);
                return OperationResult<IEnumerable<CategoryDto>>.Failure("Ett internt fel inträffade när Kategorierna skulle hämtas.");
            }
        }

        public async Task<OperationResult<CategoryDto>> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                var categoryResult = await _categoryRepository.ProductGetOneAsync(c => c.Id == categoryId);
                if (!categoryResult.IsSuccess && categoryResult.Data == null)
                {
                    return OperationResult<CategoryDto>.Failure("Kategorin kunde inte hittas.");
                }

                var categoryDto = ConvertToDto(categoryResult.Data);
                return OperationResult<CategoryDto>.Success("Kategorin hämtades framgångsrikt.", categoryDto);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR :: " + ex.Message);
                return OperationResult<CategoryDto>.Failure("Ett internt fel inträffade när Kategorin hämtades.");
            }
        }

        public async Task<OperationResult<CategoryDto>> UpdateCategoryAsync(CategoryDto categoryDto)
        {
            try
            {
                var getcategoryResult = await _categoryRepository.ProductGetOneAsync(c => c.Id == categoryDto.Id);

                if (!getcategoryResult.IsSuccess)
                {
                    return OperationResult<CategoryDto>.Failure("Kategorin kunde inte hittas.");
                }

                var entityToUpdate = getcategoryResult.Data;

                if (entityToUpdate != null)
                {
                    entityToUpdate.CategoryName = categoryDto.CategoryName;

                    var updateResult = await _categoryRepository.ProductUpdateAsync(
                        b => b.Id == entityToUpdate.Id,
                        entityToUpdate
                    );

                    if (!updateResult.IsSuccess)
                    {
                        return OperationResult<CategoryDto>.Failure("Det gick inte att uppdatera Kategorin.");
                    }
                    var updatedDto = ConvertToDto(updateResult.Data);

                    return OperationResult<CategoryDto>.Success("Kategorin uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<CategoryDto>.Failure("Kategorin kunde inte hittas.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("ERROR :: " + ex.Message);
                return OperationResult<CategoryDto>.Failure("Ett internt fel inträffade när Kategorin skulle uppdateras.");
            }
        }

        private CategoryDto ConvertToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                CategoryName = category.CategoryName
            };
        }
    }
}
