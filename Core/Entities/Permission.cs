using System;

namespace Core.Entities;

public class Permission : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public List<Role> Roles { get; set; } = new List<Role>();
}
