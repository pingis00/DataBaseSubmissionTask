using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Dtos;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface IProductReviewService
{
    Task<OperationResult<ProductReviewDto>> CreateProductReviewAsync(ProductReviewDto productReviewDto);
    Task<OperationResult<ProductReviewDto>> GetProductReviewByIdAsync(int productId);
    Task<OperationResult<IEnumerable<ProductReviewDto>>> GetAllProductReviewsAsync();
    Task<OperationResult<ProductReviewDto>> UpdateProductReviewAsync(ProductReviewDto productReviewDto);
    Task<OperationResult<bool>> DeleteProductReviewAsync(int productReviewId);
    Task<OperationResult<IEnumerable<ProductReviewDto>>> GetReviewsByProductIdAsync(int productId);
}
