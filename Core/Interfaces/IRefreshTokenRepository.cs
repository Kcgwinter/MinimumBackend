using Core.Entities;

namespace Core.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken> AddAsync(RefreshToken token);
    Task<RefreshToken?> FirstOrDefaultAsync(System.Linq.Expressions.Expression<System.Func<RefreshToken, bool>> predicate);
    Task RemoveAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
}
