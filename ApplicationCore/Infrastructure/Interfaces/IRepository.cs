using System.Linq.Expressions;

namespace ApplicationCore.Infrastructure.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T> CreateAsync(T entity);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> GetOneAsync(Expression<Func<T, bool>> predicate);
    Task<T> UpdateAsync(Expression<Func<T, bool>> predicate, T Entity);
    Task<bool> DeleteAsync(Expression<Func<T, bool>> predicate);
}
