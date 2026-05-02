using System;

namespace Infrastructure.Interfaces;

public interface IPermissionProvider
{
    Task<HashSet<string>> GetPermissionsForUserAsync(int userId);
}
