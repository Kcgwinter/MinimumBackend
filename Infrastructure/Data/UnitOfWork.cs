using System;
using System.Collections.Generic;
using System.Configuration;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Infrastructure.Repositories;

namespace Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        public IRepository<User> Users { get; private set; }
        public IRepository<TodoItem> TodoItems { get; private set; }

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            Users = new Repository<User>(_dbContext);
            TodoItems = new Repository<TodoItem>(_dbContext);
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