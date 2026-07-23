using Core.Entities;

namespace Core.Interfaces;

public interface IUserRepository
{
    Task<User> AddAsync(User user);
    Task<User?> GetByUsernameAsync(string username);
    Task<User?> GetByEmailAsync(string? email);
    Task<User?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<System.Func<User, bool>> predicate);
    Task<bool> ExistsByUsernameAsync(string username);
    Task UpdateAsync(User user);
}
