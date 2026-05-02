using System;

namespace Core.Entities;

public class Role : BaseEntity
{
    public required string Name { get; set; }
    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<Permission> Permissions { get; set; } = new List<Permission>();
}
