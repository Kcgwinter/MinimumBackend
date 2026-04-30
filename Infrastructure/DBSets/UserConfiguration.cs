using System;
using Core.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DBSets;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.HasIndex(u => u.Email).IsUnique();
        builder.HasIndex(u => u.Username).IsUnique();
        builder.Property(u => u.Email).IsRequired().HasMaxLength(100);
        builder.Property(u => u.Username).IsRequired().HasMaxLength(50);
        builder.HasQueryFilter(u => !u.IsDeleted); //global query filter to exclude soft-deleted entities
    }
}
