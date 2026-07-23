using Core.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<User> AddAsync(User user)
    {
        await context.Users.AddAsync(user);
        return user;
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await context.Users.FirstOrDefaultAsync(u => u.Username == username);
    }

    public async Task<User?> GetByEmailAsync(string? email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return null;

        return await context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<System.Func<User, bool>> predicate)
    {
        return await context.Users.FirstOrDefaultAsync(predicate);
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        return await context.Users.AnyAsync(u => u.Username == username);
    }

    public async Task UpdateAsync(User user)
    {
        context.Users.Update(user);
        await Task.CompletedTask;
    }
}
