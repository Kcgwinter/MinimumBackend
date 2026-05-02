using System;
using Core.Entities;

namespace Infrastructure.Data;

public class DBInitializer
{
    public static async Task SeedAsync(AppDbContext context)
    {
        // Ensure the database is created
        await context.Database.EnsureCreatedAsync();

        if (!context.Permissions.Any())
        {
            var permissions = new List<Permission>
            {
                new Permission { Name = "CreateUser" },
                new Permission { Name = "EditUser" },
                new Permission { Name = "DeleteUser" },
                new Permission { Name = "ViewUser" },
            };

            context.Permissions.AddRange(permissions);
            await context.SaveChangesAsync();
        }

        if (!context.Roles.Any())
        {
            var adminRole = new Role { Name = "Admin" };
            var userRole = new Role { Name = "User" };

            // Assign all permissions to Admin role
            adminRole.Permissions = context.Permissions.ToList();
            userRole.Permissions = context.Permissions.Where(p => p.Name == "ViewUser").ToList();

            context.Roles.AddRange(adminRole, userRole);
            await context.SaveChangesAsync();
        }

        if (!context.Users.Any())
        {
            var adminUser = new User { Username = "admin", Email = "admin@example.com" };
            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }
    }
}
