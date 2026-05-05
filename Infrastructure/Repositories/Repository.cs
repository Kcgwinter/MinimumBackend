// using System.Linq.Expressions;
// using Core.Entities;
// using Core.Interfaces;
// using Infrastructure.Data;
// using Microsoft.EntityFrameworkCore;

// namespace Infrastructure.Repositories
// {
//     public class Repository<T>(AppDbContext context) : IRepository<T>
//         where T : BaseEntity
//     {
//         protected readonly AppDbContext _context = context;
//         protected readonly DbSet<T> _dbSet = context.Set<T>();

//         public async Task<List<T>> GetAllAsync()
//         {
//             return await _dbSet.ToListAsync();
//         }

//         public async Task<T?> GetByIdAsync(int id)
//         {
//             return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
//         }

//         public async Task<T?> FindFirstAsync(Expression<Func<T, bool>> predicate)
//         {
//             return await _dbSet.FirstOrDefaultAsync(e =>
//                 !e.IsDeleted && predicate.Compile().Invoke(e)
//             );
//         }

//         public async Task<T> AddAsync(T entity)
//         {
//             await _dbSet.AddAsync(entity);
//             await _context.SaveChangesAsync();
//             return entity;
//         }

//         public async Task UpdateAsync(T entity)
//         {
//             _dbSet.Update(entity);
//             await _context.SaveChangesAsync();
//         }

//         public async Task DeleteAsync(T entity)
//         {
//             if (entity is BaseEntity baseEntity)
//             {
//                 baseEntity.IsDeleted = true;
//                 baseEntity.DeletedAt = DateTime.UtcNow;
//                 _dbSet.Update(entity);
//             }
//             else
//             {
//                 _dbSet.Remove(entity);
//             }
//             await _context.SaveChangesAsync();
//         }

//         /// Permanently deletes the entity from the database, bypassing soft delete.
//         public async Task ForceDeleteAsync(T entity)
//         {
//             _dbSet.Remove(entity);
//             await _context.SaveChangesAsync();
//         }

//         public async Task<bool> ExistsAsync(Expression<Func<T, bool>> predicate) =>
//             await _dbSet.AnyAsync(predicate);

//         public async Task<bool> DeletedExistsAsync(Expression<Func<T, bool>> predicate) =>
//             await _dbSet.AnyAsync(e => e.IsDeleted && predicate.Compile().Invoke(e));

//         public async Task<int> CountAsync(Expression<Func<T, bool>> predicate = null!) =>
//             predicate == null ? await _dbSet.CountAsync() : await _dbSet.CountAsync(predicate);

//         public async Task<List<T>> FindAsync(Expression<Func<T, bool>> predicate) =>
//             await _dbSet.Where(predicate).ToListAsync();
//     }
// }
