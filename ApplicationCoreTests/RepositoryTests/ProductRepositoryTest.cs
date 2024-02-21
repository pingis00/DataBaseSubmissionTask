using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Repositories;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class ProductRepositoryTest
{
    private readonly DataContext _context =
new(new DbContextOptionsBuilder<DataContext>()
.UseInMemoryDatabase($"{Guid.NewGuid()}")
.Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnProductEntityWithId_1()
    {
        var (productRepository, productEntity) = SetupProductTest();

        var result = await productRepository.ProductCreateAsync(productEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal(12547896, result.Data.ArticleNumber);
        Assert.Equal("Samsung S24", result.Data.Title);
        Assert.Equal("test.test.test", result.Data.ProductDescription);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var productRepository = new ProductRepository(_context);
        var productEntity = new Product();

        var result = await productRepository.ProductCreateAsync(productEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (productRepository, productEntity) = SetupProductTest();
        var createResult = await productRepository.ProductCreateAsync(productEntity);

        var deleteResult = await productRepository.ProductDeleteAsync(p => p.Id == createResult.Data.Id);
        var findResult = await productRepository.ProductGetOneAsync(p => p.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (productRepository, productEntity) = SetupProductTest();

        var deleteResult = await productRepository.ProductDeleteAsync(p => p.Id == productEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnCustomerReview()
    {
        var (productRepository, productEntity) = SetupProductTest();
        await productRepository.ProductCreateAsync(productEntity);

        var result = await productRepository.ProductFindAsync(p =>
        p.ArticleNumber == 12547896 &&
        p.Title == "Samsung S24" &&
        p.ProductDescription == "test.test.test");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (productRepository, productEntity) = await SetupProductWithRelationsTest();
        await productRepository.ProductCreateAsync(productEntity);

        var result = await productRepository.ProductGetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<Product>>(result.Data);
        Assert.Contains(result.Data, x => x.Id == productEntity.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (productRepository, productEntity) = await SetupProductWithRelationsTest();
        await productRepository.ProductCreateAsync(productEntity);

        var result = await productRepository.ProductGetOneAsync(c => c.Id == productEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(productEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (productRepository, productEntity) = SetupProductTest();

        var result = await productRepository.ProductGetOneAsync(p => p.Id == productEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (productRepository, productEntity) = await SetupProductWithRelationsTest();
        var createdEntity = await productRepository.ProductCreateAsync(productEntity);
        var updatedEntity = new Product
        {
            Id = createdEntity.Data.Id,
            ArticleNumber = 12547899,
            Title = "Samsung S23",
            ProductDescription = "test.test",
        };
        var findResult = await productRepository.ProductGetOneAsync(p => p.Id == createdEntity.Data.Id);

        var updateResult = await productRepository.ProductUpdateAsync(p => p.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(productEntity.Id, updateResult.Data.Id);
        Assert.Equal(12547899, updateResult.Data.ArticleNumber);
        Assert.Equal("Samsung S23", updateResult.Data.Title);
        Assert.Equal("test.test", updateResult.Data.ProductDescription);
    }

    private (ProductRepository, Product) SetupProductTest()
    {
        var productRepository = new ProductRepository(_context);
        var productEntity = new Product
        {
            ArticleNumber = 12547896,
            Title = "Samsung S24",
            ProductDescription = "test.test.test",
        };

        return (productRepository, productEntity);
    }

    private async Task<(ProductRepository, Product)> SetupProductWithRelationsTest()
    {
        var brandRepository = new BrandRepository(_context);
        var brandEntity = new Brand
        {
            BrandName = "Samsung"
        };
        var brandResult = await brandRepository.ProductCreateAsync(brandEntity);
        var createdBrand = brandResult.Data;

        var categoryRepository = new CategoryRepository(_context);
        var categoryEntity = new Category
        {
            CategoryName = "Mobil"
        };
        var categoryResult = await categoryRepository.ProductCreateAsync(categoryEntity);
        var createdCategory = categoryResult.Data;

        var productRepository = new ProductRepository(_context);
        var productEntity = new Product
        {
            ArticleNumber = 12547896,
            Title = "Samsung S24",
            ProductDescription = "test.test.test",
            BrandId = createdBrand.Id,
            CategoryId = createdCategory.Id,
        };

        return (productRepository, productEntity);
    }
}
