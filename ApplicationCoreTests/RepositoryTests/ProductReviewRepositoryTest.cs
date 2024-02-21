using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class ProductReviewRepositoryTest
{
    private readonly DataContext _context =
        new(new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task ProductCreateAsync_ShouldSaveRecordToDatabase_ReturnproductReviewEntityWithId_1()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();

        var result = await productReviewRepository.ProductCreateAsync(productReviewEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Laban", result.Data.ReviewName);
        Assert.Equal("En liten men ganska lång test ändå", result.Data.ReviewText);
    }
    [Fact]
    public async Task ProductCreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var productReviewRepository = new ProductReviewRepository(_context);
        var productReviewEntity = new ProductReview();

        var result = await productReviewRepository.ProductCreateAsync(productReviewEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task ProductDeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();
        var createResult = await productReviewRepository.ProductCreateAsync(productReviewEntity);

        var deleteResult = await productReviewRepository.ProductDeleteAsync(p => p.Id == createResult.Data.Id);
        var findResult = await productReviewRepository.ProductGetOneAsync(p => p.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task ProductDeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();

        var deleteResult = await productReviewRepository.ProductDeleteAsync(p => p.Id == productReviewEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnCustomerReview()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();
        await productReviewRepository.ProductCreateAsync(productReviewEntity);

        var result = await productReviewRepository.ProductFindAsync(c => c.ReviewText == "En liten men ganska lång test ändå");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task ProductGetAllAsync_ShouldReturnAllRecords()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();
        await productReviewRepository.ProductCreateAsync(productReviewEntity);

        var result = await productReviewRepository.ProductGetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<ProductReview>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task ProductGetOneAsync_ShouldReturnSingleRecordById()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();
        await productReviewRepository.ProductCreateAsync(productReviewEntity);

        var result = await productReviewRepository.ProductGetOneAsync(p => p.Id == productReviewEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(productReviewEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task ProductGetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();

        var result = await productReviewRepository.ProductGetOneAsync(p => p.Id == productReviewEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ProductUpdateAsync_ShouldModifyExistingRecord()
    {
        var (productReviewRepository, productReviewEntity) = await SetupProductReviewTest();
        var createdEntity = await productReviewRepository.ProductCreateAsync(productReviewEntity);
        var updatedEntity = new ProductReview
        {
            Id = createdEntity.Data.Id,
            ReviewName = "Sven",
            ReviewText = "En lång men ganska kort text ändå",
            ProductId = productReviewEntity.ProductId
        };
        var findResult = await productReviewRepository.ProductGetOneAsync(p => p.Id == createdEntity.Data.Id);

        var updateResult = await productReviewRepository.ProductUpdateAsync(p => p.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(productReviewEntity.Id, updateResult.Data.Id);
        Assert.Equal("Sven", updateResult.Data.ReviewName);
        Assert.Equal("En lång men ganska kort text ändå", updateResult.Data.ReviewText);
    }


    private async Task<(ProductReviewRepository, ProductReview)> SetupProductReviewTest()
    {
        var productRepository = new ProductRepository(_context);
        var productEntity = new Product
        {
            ArticleNumber = 12547896,
            Title = "Samsung S24",
            ProductDescription = "test.test.test",
        };

        var productResult = await productRepository.ProductCreateAsync(productEntity);
        var createdProduct = productResult.Data;

        var productReviewRepository = new ProductReviewRepository(_context);
        var productReviewEntity = new ProductReview
        {
            ReviewName = "Laban",
            ReviewText = "En liten men ganska lång test ändå",
            ProductId = createdProduct.Id,
        };

        return (productReviewRepository, productReviewEntity);
    }
}
