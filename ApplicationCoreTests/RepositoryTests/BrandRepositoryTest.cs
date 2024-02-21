using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class BrandRepositoryTest
{
    private readonly DataContext _context =
    new(new DbContextOptionsBuilder<DataContext>()
    .UseInMemoryDatabase($"{Guid.NewGuid()}")
    .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnBrandEntityWithId_1()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        var result = await brandRepository.ProductCreateAsync(brandEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Samsung", result.Data.BrandName);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var brandRepository = new BrandRepository(_context);
        var brandEntity = new Brand();

        var result = await brandRepository.ProductCreateAsync(brandEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        var createResult = await brandRepository.ProductCreateAsync(brandEntity);

        var deleteResult = await brandRepository.ProductDeleteAsync(b => b.Id == createResult.Data.Id);
        var findResult = await brandRepository.ProductGetOneAsync(b => b.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();

        var deleteResult = await brandRepository.ProductDeleteAsync(b => b.Id == brandEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnRole()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        await brandRepository.ProductCreateAsync(brandEntity);

        var result = await brandRepository.ProductFindAsync(r => r.BrandName == "Samsung");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        await brandRepository.ProductCreateAsync(brandEntity);

        var result = await brandRepository.ProductGetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<Brand>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        await brandRepository.ProductCreateAsync(brandEntity);

        var result = await brandRepository.ProductGetOneAsync(b => b.Id == brandEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(brandEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();

        var result = await brandRepository.ProductGetOneAsync(r => r.Id == brandEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        var createdEntity = await brandRepository.ProductCreateAsync(brandEntity);
        var updatedEntity = new Brand { Id = createdEntity.Data.Id, BrandName = "Apple" };
        var findResult = await brandRepository.ProductGetOneAsync(b => b.Id == createdEntity.Data.Id);

        var updateResult = await brandRepository.ProductUpdateAsync(b => b.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(brandEntity.Id, updateResult.Data.Id);
        Assert.Equal("Apple", updateResult.Data.BrandName);
    }

    [Fact]
    public async Task HasCustomersAsync_WithCustomers_ReturnsTrue()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();
        var customerRepository = new ProductRepository(_context);

        var savedBrand = await brandRepository.ProductCreateAsync(brandEntity);

        var product = new Product
        {
            ArticleNumber = 12547896,
            Title = "Samsung S24",
            ProductDescription = "test.test.test",
            BrandId = savedBrand.Data.Id
        };
        await customerRepository.ProductCreateAsync(product);
        var hasProducts = await brandRepository.HasProductsAsync(savedBrand.Data.Id);

        Assert.True(hasProducts);
    }

    [Fact]
    public async Task HasCustomersAsync_WithOutCustomers_ReturnsFalse()
    {
        var (brandRepository, brandEntity) = SetupBrandTest();

        var savedBrand = await brandRepository.ProductCreateAsync(brandEntity);

        var hasProducts = await brandRepository.HasProductsAsync(savedBrand.Data.Id);

        Assert.False(hasProducts);
    }


    public (BrandRepository, Brand) SetupBrandTest()
    {
        var brandRepository = new BrandRepository(_context);
        var brandEntity = new Brand
        {
            BrandName = "Samsung"
        };

        return (brandRepository, brandEntity);
    }
}
