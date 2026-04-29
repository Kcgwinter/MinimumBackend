using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public IRepository<User> Users { get; private set; }

        public IRepository<RefreshToken> RefreshTokens { get; private set; }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new Repository<User>(_dbContext);
            RefreshTokens = new Repository<RefreshToken>(_dbContext);
        }

        public async Task<int> CompleteAsync()
        {
            return await _dbContext.SaveChangesAsync();
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }
    }
}
