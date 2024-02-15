using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Entities;
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
            var result = await _productRepository.ProductFindAsync(p => p.ArticleNumber == product.ArticleNumber);

            if (result.IsSuccess && result.Data.Any())
            {
                await transaction.RollbackAsync();
                return OperationResult<CompleteProductDto>.Failure("Artikelnumret finns redan i systemet.");
            }
            else
            {
                var brandModel = await _brandService.CreateBrandAsync(product.Brand);
                if (!brandModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure(brandModel.Message);
                }
                product.Brand.Id= brandModel.Data.Id;

                var categoryModel = await _categoryService.CreateCategoryAsync(product.Category);
                if (!categoryModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure(categoryModel.Message);
                }
                product.Category.Id = categoryModel.Data.Id;

                var normalizedTitle = TextNormalizationHelper.NormalizeText(product.Title).Data;

                var createProductEntityResult = await _productRepository.ProductCreateAsync(new Product
                {
                    Title = normalizedTitle,
                    ProductDescription = product.ProductDescription,
                    BrandId = product.Brand.Id,
                    CategoryId = product.Category.Id,
                });

                var inventoryModel = await _inventoryService.CreateInventoryAsync(product.Inventory);
                if (!inventoryModel.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure(inventoryModel.Message);
                }
                product.Inventory.Id = inventoryModel.Data.Id;

                if (!createProductEntityResult.IsSuccess)
                {
                    await transaction.RollbackAsync();
                    return OperationResult<CompleteProductDto>.Failure("Det gick inte att skapa produktentiteten.");
                }

                var productEntity = createProductEntityResult.Data;

                var newProductDto = new CompleteProductDto
                {
                    ArticleNumber = productEntity.ArticleNumber,
                    Title = productEntity.Title,
                    ProductDescription = productEntity.ProductDescription,
                };

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
                    var result = await _productRepository.ProductDeleteAsync(p => p.ArticleNumber == productToDelete.ArticleNumber);
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
                var productsDto = productEntitiesResult.Data.Select(productEntity => new CompleteProductDto
                {
                    ArticleNumber = productEntity.ArticleNumber,
                    Title = productEntity.Title,
                    ProductDescription = productEntity.ProductDescription,
                    Brandname = productEntity.Brand.Brandname,
                    CategoryName = productEntity.Category.CategoryName,
                    Quantity = productEntity.Inventory.Quantity,
                    Price = productEntity.Inventory.Price,

                }).ToList();

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
            var productResult = await _productRepository.ProductGetOneAsync(p => p.ArticleNumber == productId);
            if (productResult.IsSuccess && productResult.Data != null)
            {
                var product = productResult.Data;

                var productDto = new CompleteProductDto
                {
                    ArticleNumber = product.ArticleNumber,
                    Title = product.Title,
                    ProductDescription = product.ProductDescription,
                    Brandname = product.Brand.Brandname,
                    CategoryName = product.Category.CategoryName,
                    Quantity = product.Inventory.Quantity,
                    Price = product.Inventory.Price

                };

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
            var getProductResult = await _productRepository.ProductGetOneAsync(p => p.ArticleNumber == updateProductDto.ArticleNumber);
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
                    entityToUpdate.Title = updateProductDto.Title;
                    entityToUpdate.ProductDescription = updateProductDto.ProductDescription;
                    entityToUpdate.BrandId = brandResult.Data.Id;
                    entityToUpdate.CategoryId = categoryResult.Data.Id;

                    var updateResult = await _productRepository.ProductUpdateAsync(
                        p => p.ArticleNumber == entityToUpdate.ArticleNumber,
                        entityToUpdate
                    );

                    if (!updateResult.IsSuccess)
                    {
                        return OperationResult<UpdateProductDto>.Failure("Det gick inte att uppdatera produkten.");
                    }
                    var updatedEntity = updateResult.Data;
                    var updatedDto = new UpdateProductDto
                    {
                        ArticleNumber = updatedEntity.ArticleNumber,
                        Title = updatedEntity.Title,
                        ProductDescription = updatedEntity.ProductDescription,
                        Brand = brandResult.Data,
                        Category = categoryResult.Data,
                    };
                    return OperationResult<UpdateProductDto>.Success("Kunden uppdaterades framgångsrikt.", updatedDto);

                }
                else
                {
                    return OperationResult<UpdateProductDto>.Failure("Kunden kunde inte hittas.");
                }

            }
            else
            {
                return OperationResult<CompleteProductDto>.Failure("Kunden kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<CompleteProductDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }
}
