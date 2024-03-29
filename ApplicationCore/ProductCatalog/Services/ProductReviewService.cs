﻿using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.ProductCatalog.Dtos;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;
using System.Diagnostics;

namespace ApplicationCore.ProductCatalog.Services;

public class ProductReviewService(IProductReviewRepository productReviewRepository, IProductService productService) : IProductReviewService
{

    private readonly IProductReviewRepository _productReviewRepository = productReviewRepository;
    private readonly IProductService _productService = productService;

    public async Task<OperationResult<ProductReviewDto>> CreateProductReviewAsync(ProductReviewDto productReviewDto)
    {
        try
        {
            var productResult = await _productService.GetProductByArticleNumberAsync(productReviewDto.Product.ArticleNumber);
            if (!productResult.IsSuccess)
            {
                return OperationResult<ProductReviewDto>.Failure("Ingen Produkt hittades med det artikelnumret");
            }

            var product = productResult.Data;
            var productReviewEntity = new ProductReview
            {
                ReviewName = productReviewDto.ReviewName,
                ReviewText = productReviewDto.ReviewText,
                ProductId = product.Id,
            };

            var reviewResult = await _productReviewRepository.ProductCreateAsync(productReviewEntity);
            if (!reviewResult.IsSuccess)
            {
                return OperationResult<ProductReviewDto>.Failure("Det gick inte att skapa recensionen.");
            }

            var createdReviewDto = ConvertToDto(reviewResult.Data);
            return OperationResult<ProductReviewDto>.Success("Recensionen skapades framgångsrikt.", createdReviewDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR :: {ex.Message}");
            return OperationResult<ProductReviewDto>.Failure("Ett internt fel inträffade när recensionen skulle skapas.");
        }
    }

    public async Task<OperationResult<bool>> DeleteProductReviewAsync(int productReviewId)
    {
        try
        {
            var reviewToDeleteResult = await GetProductReviewByIdAsync(productReviewId);

            if (!reviewToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Recensionen kunde inte hittas.");
            }
            var reviewToDelete = reviewToDeleteResult.Data;


            var result = await _productReviewRepository.ProductDeleteAsync(r => r.Id == reviewToDelete.Id);
            if (result.IsSuccess)
            {
                return OperationResult<bool>.Success("Recensionen raderades framgångsrikt.", true);
            }
            else
            {
                return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av Recensionen.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när Recensionen skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<ProductReviewDto>>> GetAllProductReviewsAsync()
    {
        try
        {
            var reviewEntitiesResult = await _productReviewRepository.ProductGetAllAsync();

            if (reviewEntitiesResult.IsSuccess && reviewEntitiesResult.Data != null)
            {


                var reviewDto = reviewEntitiesResult.Data.Select(reviewEntity => ConvertToDto(reviewEntity, true)).ToList();

                if (reviewDto.Any())
                {
                    return OperationResult<IEnumerable<ProductReviewDto>>.Success("Recensionerna hämtades framgångsrikt.", reviewDto);
                }
                else
                {
                    return OperationResult<IEnumerable<ProductReviewDto>>.Failure("Inga Recensioner hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<ProductReviewDto>>.Failure("Det gick inte att hämta Recensionerna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<ProductReviewDto>>.Failure("Ett internt fel inträffade när adresserna skulle hämtas.");
        }
    }

    public async Task<OperationResult<ProductReviewDto>> GetProductReviewByIdAsync(int productId)
    {
        try
        {
            var reviewResult = await _productReviewRepository.ProductGetOneAsync(r => r.Id == productId);
            if (!reviewResult.IsSuccess || reviewResult.Data == null)
            {
                return OperationResult<ProductReviewDto>.Failure("Recensionen kunde inte hittas.");
            }

            var reviewEntity = reviewResult.Data;

            var reviewDto = ConvertToDto(reviewEntity);
            return OperationResult<ProductReviewDto>.Success("Recensionen hämtades framgångsrikt.", reviewDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR :: {ex.Message}");
            return OperationResult<ProductReviewDto>.Failure("Ett internt fel inträffade när recensionen hämtades.");
        }
    }

    public async Task<OperationResult<IEnumerable<ProductReviewDto>>> GetReviewsByProductIdAsync(int productId)
    {
        try
        {
            var reviewEntitiesResult = await _productReviewRepository.ProductFindAsync(p => p.Id == productId);

            if (reviewEntitiesResult.IsSuccess && reviewEntitiesResult.Data != null)
            {
                var reviewDtos = reviewEntitiesResult.Data.Select(reviewEntity => new ProductReviewDto
                {
                    Id = reviewEntity.Id,
                    ReviewName = reviewEntity.ReviewName,
                    ReviewText = reviewEntity.ReviewText,
                    ProductId = reviewEntity.ProductId,
                }).ToList();

                return OperationResult<IEnumerable<ProductReviewDto>>.Success("Recensionerna för kunden hämtades framgångsrikt.", reviewDtos);
            }
            else
            {
                return OperationResult<IEnumerable<ProductReviewDto>>.Failure("Inga recensioner för kunden hittades.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<ProductReviewDto>>.Failure("Ett internt fel inträffade när recensionerna för kunden skulle hämtas.");
        }
    }

    public async Task<OperationResult<ProductReviewDto>> UpdateProductReviewAsync(ProductReviewDto productReviewDto)
    {
        try
        {
            var getReviewResult = await _productReviewRepository.ProductGetOneAsync(r => r.Id == productReviewDto.Id);

            if (!getReviewResult.IsSuccess)
            {
                return OperationResult<ProductReviewDto>.Failure("Recensionen kunde inte hittas.");
            }

            var entityToUpdate = getReviewResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.ReviewText = productReviewDto.ReviewText;

                var updateResult = await _productReviewRepository.ProductUpdateAsync(
                    r => r.Id == entityToUpdate.Id,
                    entityToUpdate
                );

                if (updateResult.IsSuccess)
                {
                    var updatedDto = ConvertToDto(updateResult.Data);
                    return OperationResult<ProductReviewDto>.Success("Recensionen uppdaterades framgångsrikt.", updatedDto);
                }
                else
                {
                    return OperationResult<ProductReviewDto>.Failure("Det gick inte att uppdatera recensionen.");
                }
            }
            else
            {
                return OperationResult<ProductReviewDto>.Failure("Adressen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<ProductReviewDto>.Failure("Ett internt fel inträffade när adressen skulle uppdateras.");
        }
    }

    private ProductReviewDto ConvertToDto(ProductReview reviewEntity, bool previewReviewText = false)
    {
        const int previewLength = 40;
        string reviewText = previewReviewText && reviewEntity.ReviewText.Length > previewLength
                         ? reviewEntity.ReviewText.Substring(0, previewLength) + "..."
                         : reviewEntity.ReviewText;
        return new ProductReviewDto
        {
            Id = reviewEntity.Id,
            ReviewText = reviewText,
            ReviewName = reviewEntity.ReviewName,
            ProductId = reviewEntity.ProductId,
            Product = reviewEntity.Product != null ? new ProductDto
            {
                Id = reviewEntity.Product.Id,
                ArticleNumber = reviewEntity.Product.ArticleNumber,
                Title = reviewEntity.Product.Title
            } : null!
        };
    }
}
