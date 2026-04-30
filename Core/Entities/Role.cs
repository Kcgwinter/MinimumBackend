using System;

namespace Core.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public List<Permission> Permissions { get; set; } = new List<Permission>();
}
