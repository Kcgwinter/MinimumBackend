using Core.Interfaces;
using Core.Entities;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class RefreshTokenRepository(AppDbContext context) : IRefreshTokenRepository
{
    public async Task<RefreshToken> AddAsync(RefreshToken token)
    {
        await context.RefreshTokens.AddAsync(token);
        return token;
    }

    public async Task<RefreshToken?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<System.Func<RefreshToken, bool>> predicate)
    {
        return await context.RefreshTokens.FirstOrDefaultAsync(predicate);
    }

    public async Task RemoveAsync(RefreshToken token)
    {
        context.RefreshTokens.Remove(token);
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        context.RefreshTokens.Update(token);
        await Task.CompletedTask;
    }
}
