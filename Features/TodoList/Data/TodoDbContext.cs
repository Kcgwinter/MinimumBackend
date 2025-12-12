using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Features.TodoList.Models;

namespace Features.TodoList.Data
{
    public class TodoDbContext(DbContextOptions<TodoDbContext> options)  : DbContext(options)
    {
        public DbSet<TodoItem> TodoItems => Set<TodoItem>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<TodoItem>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
                entity.Property(e => e.IsCompleted).IsRequired();
            });
        }
    }
}