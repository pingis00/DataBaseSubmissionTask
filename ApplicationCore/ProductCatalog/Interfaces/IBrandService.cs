using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Dtos;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface IBrandService
{
    Task<OperationResult<BrandDto>> CreateBrandAsync(BrandDto brand);
    Task<OperationResult<BrandDto>> GetBrandsByIdAsync(int brandId);
    Task<OperationResult<IEnumerable<BrandDto>>> GetAllBrandsAsync();
    Task<OperationResult<BrandDto>> UpdateBrandAsync(BrandDto brandDto);
    Task<OperationResult<bool>> DeleteBrandAsync(int brandId);
    Task<OperationResult<bool>> BrandHasProductsAsync(int brandId);
}
