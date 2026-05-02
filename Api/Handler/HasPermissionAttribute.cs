using System;
using Microsoft.AspNetCore.Authorization;

namespace Api.Handler;

public class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(string permission)
        : base(policy: permission) // This 'policy' name is what Step 5 uses
    { }
}
