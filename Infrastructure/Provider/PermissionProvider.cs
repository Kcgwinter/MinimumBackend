using System;
using Infrastructure.Data;
using Infrastructure.Interfaces;

namespace Infrastructure.Provider;

public class PermissionProvider : IPermissionProvider
{
    private readonly AppDbContext _context;

    public PermissionProvider(AppDbContext context)
    {
        _context = context;
    }

    public Task<HashSet<string>> GetPermissionsForUserAsync(int userId)
    {
        var permissions = _context
            .Users.Where(u => u.Id == userId)
            .SelectMany(u => u.Roles)
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .Distinct()
            .ToHashSet();

        return Task.FromResult(permissions);
    }
}
