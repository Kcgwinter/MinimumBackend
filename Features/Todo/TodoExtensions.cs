using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Features.Todo.Controllers;
using Features.Todo.Data;
using Features.Todo.Interfaces;
using Features.Todo.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Features.Todo
{
    public static class TodoExtensions
    {
        public static IServiceCollection AddTodoFeature(
        this IServiceCollection services,
        string connectionString)
        {
            // Register TodoList DbContext with shared connection string
            services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlite(connectionString)
    );
            Console.WriteLine("Todo Feature Registered with Connection String: " + connectionString);
            // Register TodoList services
            services.AddScoped<ITodoService, TodoService>();
            services.AddScoped<TodoService>();

            services.AddControllers()
    .AddApplicationPart(typeof(TodoController).Assembly);

            return services;
        }
    }
}