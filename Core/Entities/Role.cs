using System;

namespace Core.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
    public ICollection<User> Users { get; set; } = new List<User>();
}
