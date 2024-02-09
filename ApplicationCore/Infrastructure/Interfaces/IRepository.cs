using ApplicationCore.Business.Helpers;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ApplicationCore.Infrastructure.Interfaces;

public interface IRepository<T> where T : class
{
    Task<OperationResult<T>> CreateAsync(T entity);
    Task<OperationResult<IEnumerable<T>>> GetAllAsync();
    Task<OperationResult<T>> GetOneAsync(Expression<Func<T, bool>> predicate);
    Task<OperationResult<T>> UpdateAsync(Expression<Func<T, bool>> predicate, T Entity);
    Task<OperationResult<bool>> DeleteAsync(Expression<Func<T, bool>> predicate);
    Task<OperationResult<IEnumerable<T>>> FindAsync(Expression<Func<T, bool>> predicate);
}
