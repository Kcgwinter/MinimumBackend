using System.Linq.Expressions;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IRepository<T>
        where T : BaseEntity
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T> AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(T entity);
        Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate);
        Task<int> CountAsync(Expression<Func<T, bool>> predicate = null!);
        Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate);
    }
}
