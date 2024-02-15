using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Dtos;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface ICategoryService
{
    Task<OperationResult<CategoryDto>> CreateCategoryAsync(CategoryDto category);
    Task<OperationResult<CategoryDto>> GetCategoryByIdAsync(int categoryId);
    Task<OperationResult<IEnumerable<CategoryDto>>> GetAllCategoriesAsync();
    Task<OperationResult<CategoryDto>> UpdateCategoryAsync(CategoryDto categoryDto);
    Task<OperationResult<bool>> DeleteCategoryAsync(int categoryId);
    Task<OperationResult<bool>> CategoryHasProductsAsync(int categoryId);
}
