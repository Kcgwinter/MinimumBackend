using System;
using System.Security.Claims;
using Infrastructure.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Api.Handler;

public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
{
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public PermissionHandler(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement
    )
    {
        // Extract the UserId from the JWT NameIdentifier claim
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (!Int32.TryParse(userIdClaim, out var userId))
            return;

        // Create a scope to resolve the Scoped DbContext/Provider
        using var scope = _serviceScopeFactory.CreateScope();
        var permissionProvider = scope.ServiceProvider.GetRequiredService<IPermissionProvider>();

        var permissions = await permissionProvider.GetPermissionsForUserAsync(userId);

        if (permissions.Contains(requirement.Permission))
        {
            context.Succeed(requirement);
        }
    }
}
