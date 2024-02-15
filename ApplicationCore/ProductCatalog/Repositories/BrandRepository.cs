using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Entities;
using ApplicationCore.ProductCatalog.Interfaces;

namespace ApplicationCore.ProductCatalog.Repositories;

public class BrandRepository(DataContext context) : BaseProductRepository<Brand>(context), IBrandRepository
{
    private readonly DataContext _dataContext = context;
}
