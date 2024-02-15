using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Dtos;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface IProductService
{
    Task<OperationResult<CompleteProductDto>> CreateProductAsync(CompleteProductDto product);
    Task<OperationResult<CompleteProductDto>> GetProductByIdAsync(int productId);
    Task<OperationResult<IEnumerable<CompleteProductDto>>> GetAllProductsAsync();
    Task<OperationResult<CompleteProductDto>> UpdateProductAsync(CompleteProductDto updateProductDto);
    Task<OperationResult<bool>> DeleteProductAsync(int productId);
}
