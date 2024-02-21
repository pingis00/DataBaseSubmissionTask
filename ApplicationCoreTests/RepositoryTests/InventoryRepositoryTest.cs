using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class InventoryRepositoryTest
{
    private readonly DataContext _context =
new(new DbContextOptionsBuilder<DataContext>()
.UseInMemoryDatabase($"{Guid.NewGuid()}")
.Options);

    [Fact]
    public async Task ProductCreateAsync_ShouldSaveRecordToDatabase_ReturnInventoryEntityWithId_1()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();

        var result = await inventoryRepository.ProductCreateAsync(inventoryEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.ProductId);
        Assert.Equal(200, result.Data.Price);
        Assert.Equal(20, result.Data.Quantity);
    }
    [Fact]
    public async Task ProductCreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var inventoryRepository = new InventoryRepository(_context);
        var inventoryEntity = new Inventory();

        var result = await inventoryRepository.ProductCreateAsync(inventoryEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task ProductDeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();
        var createResult = await inventoryRepository.ProductCreateAsync(inventoryEntity);

        var deleteResult = await inventoryRepository.ProductDeleteAsync(i => i.ProductId == createResult.Data.ProductId);
        var findResult = await inventoryRepository.ProductGetOneAsync(i => i.ProductId == createResult.Data.ProductId);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task ProductDeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();

        var deleteResult = await inventoryRepository.ProductDeleteAsync(i => i.ProductId == inventoryEntity.ProductId);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnCustomerReview()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();
        await inventoryRepository.ProductCreateAsync(inventoryEntity);

        var result = await inventoryRepository.ProductFindAsync(c => c.Price == 200);

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task ProductGetAllAsync_ShouldReturnAllRecords()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();
        await inventoryRepository.ProductCreateAsync(inventoryEntity);

        var result = await inventoryRepository.ProductGetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<Inventory>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task ProductGetOneAsync_ShouldReturnSingleRecordById()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();
        await inventoryRepository.ProductCreateAsync(inventoryEntity);

        var result = await inventoryRepository.ProductGetOneAsync(i => i.ProductId == inventoryEntity.ProductId);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(inventoryEntity.ProductId, result.Data.ProductId);
    }

    [Fact]
    public async Task ProductGetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();

        var result = await inventoryRepository.ProductGetOneAsync(i => i.ProductId == inventoryEntity.ProductId);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task ProductUpdateAsync_ShouldModifyExistingRecord()
    {
        var (inventoryRepository, inventoryEntity) = await SetupInventoryTest();
        var createdEntity = await inventoryRepository.ProductCreateAsync(inventoryEntity);
        var updatedEntity = new Inventory
        {
            ProductId = createdEntity.Data.ProductId,
            Price = 300,
            Quantity = 30,
        };
        var findResult = await inventoryRepository.ProductGetOneAsync(i => i.ProductId == createdEntity.Data.ProductId);

        var updateResult = await inventoryRepository.ProductUpdateAsync(i => i.ProductId == createdEntity.Data.ProductId, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(inventoryEntity.ProductId, updateResult.Data.ProductId);
        Assert.Equal(300, updateResult.Data.Price);
        Assert.Equal(30, updateResult.Data.Quantity);
    }


    private async Task<(InventoryRepository, Inventory)> SetupInventoryTest()
    {
        var productRepository = new ProductRepository(_context);
        var productEntity = new Product
        {
            ArticleNumber = 12547896,
            Title = "Samsung S24",
            ProductDescription = "test.test.test",
            BrandId = 1,
            CategoryId = 1,
        };
        var productResult = await productRepository.ProductCreateAsync(productEntity);
        var createdProduct = productResult.Data;

        var inventoryRepository = new InventoryRepository(_context);
        var inventoryEntity = new Inventory
        {
            ProductId = createdProduct.Id,
            Price = 200,
            Quantity = 20
        };
        return (inventoryRepository, inventoryEntity);
    }
}
