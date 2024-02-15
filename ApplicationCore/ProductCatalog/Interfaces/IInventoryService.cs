using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Dtos;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface IInventoryService
{
    Task<OperationResult<InventoryDto>> CreateInventoryAsync(InventoryDto inventory);
    Task<OperationResult<InventoryDto>> GetInventoriesByIdAsync(int inventoryId);
    Task<OperationResult<IEnumerable<InventoryDto>>> GetAllInventoriesAsync();
    Task<OperationResult<InventoryDto>> UpdateInventoryAsync(InventoryDto inventoryDto);
    Task<OperationResult<bool>> DeleteInventoryAsync(int inventoryId);
}
