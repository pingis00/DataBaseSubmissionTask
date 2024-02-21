using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ApplicationCoreTests.RepositoryTests;

public class CategoryRepositoryTest
{
    private readonly DataContext _context =
        new(new DbContextOptionsBuilder<DataContext>()
        .UseInMemoryDatabase($"{Guid.NewGuid()}")
        .Options);

    [Fact]
    public async Task CreateAsync_ShouldSaveRecordToDatabase_ReturnCategoryEntityWithId_1()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        var result = await categoryRepository.ProductCreateAsync(categoryEntity);

        Assert.NotNull(result);
        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.NotEqual(0, result.Data.Id);
        Assert.Equal("Mobil", result.Data.CategoryName);
    }

    [Fact]
    public async Task CreateAsync_ShouldNotSaveRecordToDatabase_ReturnNull()
    {
        var categoryRepository = new CategoryRepository(_context);
        var categoryEntity = new Category();

        var result = await categoryRepository.ProductCreateAsync(categoryEntity);

        Assert.False(result.IsSuccess);
        Assert.Null(result.Data);
        Assert.NotEmpty(result.Message);
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveRecordFromDatabase()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        var createResult = await categoryRepository.ProductCreateAsync(categoryEntity);

        var deleteResult = await categoryRepository.ProductDeleteAsync(c => c.Id == createResult.Data.Id);
        var findResult = await categoryRepository.ProductGetOneAsync(c => c.Id == createResult.Data.Id);

        Assert.True(deleteResult.IsSuccess);
        Assert.Null(findResult.Data);
    }

    [Fact]
    public async Task DeleteAsync_ShouldNotRemoveRecordFromDatabase()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();

        var deleteResult = await categoryRepository.ProductDeleteAsync(c => c.Id == categoryEntity.Id);

        Assert.False(deleteResult.IsSuccess);
    }

    [Fact]
    public async Task FindAsync_ShouldReturnFilteredRecordsBasedOnRole()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        await categoryRepository.ProductCreateAsync(categoryEntity);

        var result = await categoryRepository.ProductFindAsync(r => r.CategoryName == "Mobil");

        Assert.True(result.IsSuccess);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetAllAsync_ShouldReturnAllRecords()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        await categoryRepository.ProductCreateAsync(categoryEntity);

        var result = await categoryRepository.ProductGetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.NotEmpty(result.Data);
        Assert.IsAssignableFrom<IEnumerable<Category>>(result.Data);
        Assert.Single(result.Data);
    }

    [Fact]
    public async Task GetOneAsync_ShouldReturnSingleRecordById()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        await categoryRepository.ProductCreateAsync(categoryEntity);

        var result = await categoryRepository.ProductGetOneAsync(c => c.Id == categoryEntity.Id);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Data);
        Assert.Equal(categoryEntity.Id, result.Data.Id);
    }

    [Fact]
    public async Task GetOneAsync_ShouldNotReturnSingleRecordById_ReturnNull()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();

        var result = await categoryRepository.ProductGetOneAsync(c => c.Id == categoryEntity.Id);

        Assert.Null(result.Data);
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingRecord()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        var createdEntity = await categoryRepository.ProductCreateAsync(categoryEntity);
        var updatedEntity = new Category { Id = createdEntity.Data.Id, CategoryName = "Dator" };
        var findResult = await categoryRepository.ProductGetOneAsync(c => c.Id == createdEntity.Data.Id);

        var updateResult = await categoryRepository.ProductUpdateAsync(c => c.Id == createdEntity.Data.Id, updatedEntity);

        Assert.NotNull(findResult.Data);
        Assert.NotNull(updateResult.Data);
        Assert.Equal(categoryEntity.Id, updateResult.Data.Id);
        Assert.Equal("Dator", updateResult.Data.CategoryName);
    }

    [Fact]
    public async Task HasCustomersAsync_WithCustomers_ReturnsTrue()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();
        var customerRepository = new ProductRepository(_context);

        var savedCategory = await categoryRepository.ProductCreateAsync(categoryEntity);

        var product = new Product
        {
            ArticleNumber = 12547896,
            Title = "Samsung S24",
            ProductDescription = "test.test.test",
            CategoryId = savedCategory.Data.Id
        };
        await customerRepository.ProductCreateAsync(product);
        var hasProducts = await categoryRepository.HasProductsAsync(savedCategory.Data.Id);

        Assert.True(hasProducts);
    }

    [Fact]
    public async Task HasCustomersAsync_WithOutCustomers_ReturnsFalse()
    {
        var (categoryRepository, categoryEntity) = SetupCategoryTest();

        var savedCategory = await categoryRepository.ProductCreateAsync(categoryEntity);

        var hasProducts = await categoryRepository.HasProductsAsync(savedCategory.Data.Id);

        Assert.False(hasProducts);
    }


    public (CategoryRepository, Category) SetupCategoryTest()
    {
        var categoryRepository = new CategoryRepository(_context);
        var categoryEntity = new Category
        {
            CategoryName = "Mobil"
        };

        return (categoryRepository, categoryEntity);
    }
}
