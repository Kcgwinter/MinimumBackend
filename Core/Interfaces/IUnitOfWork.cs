using Core.Entities;

namespace Core.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<RefreshToken> RefreshTokens { get; }

        Task<int> CompleteAsync();
    }
}
