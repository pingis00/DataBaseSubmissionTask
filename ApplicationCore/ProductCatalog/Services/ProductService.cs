using ApplicationCore.Business.Helpers;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using System.Diagnostics;

namespace ApplicationCore.ProductCatalog.Services;

public class ProductService(IProductRepository productRepository, IBrandService brandService, ICategoryService categoryService, IInventoryService inventoryService, DataContext dataContext) : IProductService
{
    private readonly IProductRepository _productRepository = productRepository;
    private readonly IBrandService _brandService = brandService;
    private readonly ICategoryService _categoryService = categoryService;
    private readonly IInventoryService _inventoryService = inventoryService;
    private readonly DataContext _dataContext = dataContext;

    public async Task<OperationResult<CompleteProductDto>> CreateProductAsync(CompleteProductDto product)
    {
        using var transaction = _dataContext.Database.BeginTransaction();
        try
        {
            var result = await _productRepository.ProductFindAsync(p => p.Id == product.Id);

            if (result.IsSuccess && result.Data.Any())
            {
                await transaction.RollbackAsync();
                return OperationResult<CompleteProductDto>.Failure("Artikelnumret finns redan i systemet.");
            }
            else
            {
                var brandResult = await _brandService.CreateBrandAsync(product.Brand);
                if (!brandResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure(brandResult.Message);
                }
                product.Brand.Id= brandResult.Data.Id;

                var categoryResult = await _categoryService.CreateCategoryAsync(product.Category);
                if (!categoryResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure(categoryResult.Message);
                }
                product.Category.Id = categoryResult.Data.Id;

                var normalizedTitle = TextNormalizationHelper.NormalizeText(product.Title).Data;

                var createProductResult = await _productRepository.ProductCreateAsync(new Product
                {
                    ArticleNumber = product.ArticleNumber,
                    Title = normalizedTitle,
                    ProductDescription = product.ProductDescription,
                    BrandId = product.Brand.Id,
                    CategoryId = product.Category.Id,
                });
                product.Inventory.ProductId = createProductResult.Data.Id;

                var inventoryResult = await _inventoryService.CreateInventoryAsync(product.Inventory);
                if (!inventoryResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure(inventoryResult.Message);
                }

                var newProductDto = ConvertToCompleteProductDto(createProductResult.Data, brandResult.Data, categoryResult.Data, inventoryResult.Data);

                await transaction.CommitAsync();
                return OperationResult<CompleteProductDto>.Success("Produkten skapades framgångrikt", newProductDto);
            }
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CompleteProductDto>.Failure("Ett internt fel inträffade när produkten hämtades.");
        }
    }

    public async Task<OperationResult<bool>> DeleteProductAsync(int productId)
    {
        try
        {
            var productToDeleteResult = await GetProductByIdAsync(productId);

            if (!productToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Produkten kunde inte hittas.");
            }

            var productToDelete = productToDeleteResult.Data;
            {
                if (productToDelete != null)
                {
                    var result = await _productRepository.ProductDeleteAsync(p => p.Id == productToDelete.Id);
                    if (result.IsSuccess)
                    {
                        return OperationResult<bool>.Success("Produkten raderades framgångsrikt.", true);
                    }
                    else
                    {
                        return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av produkten.");
                    }
                }
                else
                {
                    return OperationResult<bool>.Failure("Produkten kunde inte hittas.");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när produkten skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<CompleteProductDto>>> GetAllProductsAsync()
    {
        try
        {
            var productEntitiesResult = await _productRepository.ProductGetAllAsync();

            if (productEntitiesResult.IsSuccess && productEntitiesResult.Data != null)
            {
                var productsDto = productEntitiesResult.Data.Select(ConvertToCompleteProductDtoFromEntity).ToList();

                if (productsDto.Any())
                {
                    return OperationResult<IEnumerable<CompleteProductDto>>.Success("Produkterna hämtades framgångsrikt.", productsDto);
                }
                else
                {
                    return OperationResult<IEnumerable<CompleteProductDto>>.Failure("Inga produkter hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<CompleteProductDto>>.Failure("Det gick inte att hämta produkterna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<CompleteProductDto>>.Failure("Ett internt fel inträffade när produkterna skulle hämtas.");
        }
    }

    public async Task<OperationResult<CompleteProductDto>> GetProductByIdAsync(int productId)
    {
        try
        {
            var productResult = await _productRepository.ProductGetOneAsync(p => p.Id == productId);
            if (productResult.IsSuccess && productResult.Data != null)
            {
                var productDto = ConvertToCompleteProductDtoFromEntity(productResult.Data);

                return OperationResult<CompleteProductDto>.Success("Kunden hämtades framgångsrikt.", productDto);
            }
            else
            {
                return OperationResult<CompleteProductDto>.Failure("Kunden kunde inte hittas.");
            }
        }

        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CompleteProductDto>.Failure("Ett internt fel inträffade när kunden hämtades.");
        }
    }

    public async Task<OperationResult<UpdateProductDto>> UpdateProductAsync(UpdateProductDto updateProductDto)
    {
        try
        {
            var getProductResult = await _productRepository.ProductGetOneAsync(p => p.Id == updateProductDto.Id);
            if (!getProductResult.IsSuccess)
            {
                return OperationResult<UpdateProductDto>.Failure("Produkten kunde inte hittas.");
            }

            var entityToUpdate = getProductResult.Data;

            if (entityToUpdate != null)
            {
                var brandResult = await _brandService.CreateBrandAsync(updateProductDto.Brand);
                if (!brandResult.IsSuccess)
                {
                    return OperationResult<UpdateProductDto>.Failure("Varumärket kunde inte Uppdateras.");
                }

                var categoryResult = await _categoryService.CreateCategoryAsync(updateProductDto.Category);
                if (!categoryResult.IsSuccess)
                {
                    return OperationResult<UpdateProductDto>.Failure("Kategorin kunde inte Uppdateras.");
                }

                entityToUpdate = getProductResult.Data;

                if (entityToUpdate != null)
                {
                    entityToUpdate.ArticleNumber = updateProductDto.ArticleNumber;
                    entityToUpdate.Title = updateProductDto.Title;
                    entityToUpdate.ProductDescription = updateProductDto.ProductDescription;
                    entityToUpdate.BrandId = brandResult.Data.Id;
                    entityToUpdate.CategoryId = categoryResult.Data.Id;

                    var updateResult = await _productRepository.ProductUpdateAsync(
                        p => p.Id == entityToUpdate.Id,
                        entityToUpdate
                    );

                    if (!updateResult.IsSuccess)
                    {
                        return OperationResult<UpdateProductDto>.Failure("Det gick inte att uppdatera produkten.");
                    }
                    var updatedEntity = updateResult.Data;
                    var updatedDto = ConvertToUpdateProductDto(updatedEntity, brandResult.Data, categoryResult.Data);
                    return OperationResult<UpdateProductDto>.Success("Kunden uppdaterades framgångsrikt.", updatedDto);

                }
                else
                {
                    return OperationResult<UpdateProductDto>.Failure("Kunden kunde inte hittas.");
                }

            }
            else
            {
                return OperationResult<UpdateProductDto>.Failure("Kunden kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<UpdateProductDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }

    public UpdateProductDto ConvertToUpdatable(CompleteProductDto completeDto)
    {
        return new UpdateProductDto
        {
            Id = completeDto.Id,
            ArticleNumber = completeDto.ArticleNumber,
            Title = completeDto.Title,
            ProductDescription = completeDto.ProductDescription,
            BrandName = completeDto.BrandName,
            CategoryName = completeDto.CategoryName,
            Brand = completeDto.Brand,
            Category = completeDto.Category
        };
    }

    public async Task<OperationResult<ProductDto>> GetProductByArticleNumberAsync(int articleNumber)
    {
        try
        {
            var productResult = await _productRepository.ProductGetOneAsync(p => p.ArticleNumber == articleNumber);
            if (productResult.IsSuccess && productResult.Data != null)
            {
                var product = productResult.Data;
                var productDto = new ProductDto
                {
                    Id = product.Id,
                    ArticleNumber = product.ArticleNumber,
                    Title = product.Title,
                    ProductDescription = product.ProductDescription,
                };
                return OperationResult<ProductDto>.Success("Produkten hittades.", productDto);
            }
            else
            {
                return OperationResult<ProductDto>.Failure("Inget produkt med det artikelnumret hittades.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR :: {ex.Message}");
            return OperationResult<ProductDto>.Failure("Ett internt fel inträffade när artikelnumret skulle hämtas.");
        }
    }

    private CompleteProductDto ConvertToCompleteProductDto(Product product, BrandDto brand, CategoryDto category, InventoryDto inventory)
    {
        return new CompleteProductDto
        {
            ArticleNumber = product.ArticleNumber,
            Title = product.Title,
            ProductDescription = product.ProductDescription,
            Brand = brand,
            Category = category,
            Inventory = inventory
        };
    }

    private CompleteProductDto ConvertToCompleteProductDtoFromEntity(Product productEntity)
    {
        return new CompleteProductDto
        {
            Id = productEntity.Id,
            ArticleNumber = productEntity.ArticleNumber,
            Title = productEntity.Title,
            ProductDescription = productEntity.ProductDescription,
            Brand = new BrandDto { BrandName = productEntity.Brand.BrandName },
            Category = new CategoryDto { CategoryName = productEntity.Category.CategoryName },
            Inventory = new InventoryDto { Quantity = productEntity.Inventory?.Quantity ?? 0, Price = productEntity.Inventory?.Price ?? 0m }
        };
    }

    private UpdateProductDto ConvertToUpdateProductDto(Product updatedEntity, BrandDto brand, CategoryDto category)
    {
        return new UpdateProductDto
        {
            Id = updatedEntity.Id,
            ArticleNumber = updatedEntity.ArticleNumber,
            Title = updatedEntity.Title,
            ProductDescription = updatedEntity.ProductDescription,
            Brand = brand,
            Category = category
        };
    }


}
