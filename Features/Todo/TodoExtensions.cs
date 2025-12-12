using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Features.Todo
{
    public class TodoExtensions
    {
        public static IServiceCollection AddTodoListModule(
        this IServiceCollection services,
        string connectionString)
        {
            // Register TodoList DbContext with shared connection string
            services.AddDbContext<TodoDbContext>(options =>
                options.UseSqlServer(connectionString));

            // Register TodoList services
            services.AddScoped<ITodoService, TodoService>();

            return services;
        }
    }
}