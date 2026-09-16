using System.Text.Json;
using Kartabl_Backend.Domain.Entities;
using Kartabl_Backend.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartabl_Backend.Infrastructure.Persistence.Configurations;

public class RoleConfiguration : IEntityTypeConfiguration<Role>
{
    public void Configure(EntityTypeBuilder<Role> builder)
    {
        builder.ToTable("Roles");

        // Primary Key mapped directly from UserRole Enum (int)
        builder.HasKey(r => r.Id);

        // Store enum as underlying int in database
        builder.Property(r => r.Id)
            .HasConversion<int>()
            .ValueGeneratedNever();

        // Convert List<Permission> to/from JSON array in database
        builder.Property(r => r.Permissions)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null),
                v => (IReadOnlyCollection<Permission>)(JsonSerializer.Deserialize<List<Permission>>(v, (JsonSerializerOptions?)null) 
                                                       ?? new List<Permission>())
            )
            .HasColumnType("nvarchar(max)")
            .IsRequired()
            .Metadata.SetValueComparer(
                new ValueComparer<IReadOnlyCollection<Permission>>(
                    (c1, c2) => c1 != null && c2 != null && c1.SequenceEqual(c2),
                    c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                    c => c.ToList().AsReadOnly()
                )
            );
    }
}