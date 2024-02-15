using ApplicationCore.Business.Helpers;
using System.Linq.Expressions;

namespace ApplicationCore.ProductCatalog.Interfaces;

public interface IDataRepository<T> where T : class
{
    Task<OperationResult<T>> ProductCreateAsync(T entity);
    Task<OperationResult<IEnumerable<T>>> ProductGetAllAsync();
    Task<OperationResult<T>> ProductGetOneAsync(Expression<Func<T, bool>> predicate);
    Task<OperationResult<T>> ProductUpdateAsync(Expression<Func<T, bool>> predicate, T Entity);
    Task<OperationResult<bool>> ProductDeleteAsync(Expression<Func<T, bool>> predicate);
    Task<OperationResult<IEnumerable<T>>> ProductFindAsync(Expression<Func<T, bool>> predicate);
}
