using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using System.Data;
using System.Diagnostics;

namespace ApplicationCore.ProductCatalog.Services;

public class InventoryService(IIventoryRepository inventoryRepository) : IInventoryService
{
    private readonly IIventoryRepository _inventoryRepository = inventoryRepository;

    public async Task<OperationResult<InventoryDto>> CreateInventoryAsync(InventoryDto inventory)
    {
        try
        {
            var existingInventoryResult = await _inventoryRepository.ProductGetOneAsync(i => i.ProductId == inventory.ProductId);

            if (existingInventoryResult.IsSuccess && existingInventoryResult.Data != null)
            {
                var inventoryDto = new InventoryDto
                {
                    Id = existingInventoryResult.Data.ProductId,
                    Quantity = existingInventoryResult.Data.Quantity,
                    Price = existingInventoryResult.Data.Price,

                };

                return OperationResult<InventoryDto>.Success("Inventariet finns redan i systemet.", inventoryDto);
            }
            else
            {
                var newInventoryEntityResult = await _inventoryRepository.ProductCreateAsync(new Inventory
                {
                    ProductId = inventory.ProductId,
                    Quantity = inventory.Quantity,
                    Price = inventory.Price,
                });

                if (!newInventoryEntityResult.IsSuccess)
                {
                    return OperationResult<InventoryDto>.Failure("Det gick inte att skapa inventariet.");
                }
                var newInventoryEntity = newInventoryEntityResult.Data;

                var newInventoryDto = ConvertToDto(newInventoryEntityResult.Data);

                return OperationResult<InventoryDto>.Success("Inventariet skapades framgångrikt", newInventoryDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<InventoryDto>.Failure("Ett internt fel inträffade när Inventariet skulle skapas.");
        }
    }

    public Task<OperationResult<bool>> DeleteInventoryAsync(int inventoryId)
    {
        throw new NotImplementedException();
    }

    public async Task<OperationResult<IEnumerable<InventoryDto>>> GetAllInventoriesAsync()
    {
        try
        {
            var inventoryEntitiesResult = await _inventoryRepository.ProductGetAllAsync();

            if (inventoryEntitiesResult.IsSuccess && inventoryEntitiesResult.Data != null)
            {
                var inventoriesDto = inventoryEntitiesResult.Data.Select(ConvertToDetailedDto).ToList();

                if (inventoriesDto.Any())
                {
                    return OperationResult<IEnumerable<InventoryDto>>.Success("Inventarierna hämtades framgångsrikt.", inventoriesDto);
                }
                else
                {
                    return OperationResult<IEnumerable<InventoryDto>>.Failure("Inga inventarier hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<InventoryDto>>.Failure("Det gick inte att hämta inventarierna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<InventoryDto>>.Failure("Ett internt fel inträffade när inventarierna skulle hämtas.");
        }
    }

    public async Task<OperationResult<InventoryDto>> GetInventoriesByIdAsync(int inventoryId)
    {
        try
        {
            var inventoryResult = await _inventoryRepository.ProductGetOneAsync(i => i.ProductId == inventoryId);
            if (inventoryResult.IsSuccess && inventoryResult.Data != null)
            {
                var inventoryto = ConvertToDetailedDto(inventoryResult.Data);
                return OperationResult<InventoryDto>.Success("Inventariet hämtades framgångsrikt.", inventoryto);
            }
            else
            {
                return OperationResult<InventoryDto>.Failure("Inventariet kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<InventoryDto>.Failure("Ett internt fel inträffade när inventariet hämtades.");
        }
    }

    public async Task<OperationResult<InventoryDto>> UpdateInventoryAsync(InventoryDto inventoryDto)
    {
        try
        {
            var getInventoryResult = await _inventoryRepository.ProductGetOneAsync(i => i.ProductId == inventoryDto.ProductId);

            if (!getInventoryResult.IsSuccess)
            {
                return OperationResult<InventoryDto>.Failure("Inventariet kunde inte hittas.");
            }

            var entityToUpdate = getInventoryResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.Quantity = inventoryDto.Quantity;
                entityToUpdate.Price = inventoryDto.Price;

                var updateResult = await _inventoryRepository.ProductUpdateAsync(
                    i => i.ProductId == entityToUpdate.ProductId,
                    entityToUpdate
                );

                if (!updateResult.IsSuccess)
                {
                    return OperationResult<InventoryDto>.Failure("Det gick inte att uppdatera inventariet.");
                }
                var updatedDto = ConvertToDto(updateResult.Data);

                return OperationResult<InventoryDto>.Success("Inventariet uppdaterades framgångsrikt.", updatedDto);

            }
            else
            {
                return OperationResult<InventoryDto>.Failure("Inventariet kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<InventoryDto>.Failure("Ett internt fel inträffade när inventariet skulle uppdateras.");
        }
    }

    private InventoryDto ConvertToDto(Inventory inventory)
    {
        return new InventoryDto
        {
            Id = inventory.ProductId,
            Price = inventory.Price,
            Quantity = inventory.Quantity,
        };
    }

    private InventoryDto ConvertToDetailedDto(Inventory inventory)
    {
        return new InventoryDto
        {
            Id = inventory.ProductId,
            Quantity = inventory.Quantity,
            Price = inventory.Price,
            ProductId = inventory.Product.Id,
            Product = new ProductDto
            {
                Id= inventory.ProductId,
                ArticleNumber = inventory.Product.ArticleNumber,
                Title = inventory.Product.Title,
            }
        };
    }
}
