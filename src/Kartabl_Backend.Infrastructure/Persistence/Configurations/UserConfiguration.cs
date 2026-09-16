using Kartabl_Backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Kartabl_Backend.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("Users");

        builder.HasKey(u => u.Id);

        builder.Property(u => u.NationalCode)
            .HasMaxLength(10)
            .IsRequired();

        // Unique Index on NationalCode
        builder.HasIndex(u => u.NationalCode)
            .IsUnique();

        builder.Property(u => u.FirstName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.LastName)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.IsActive)
            .HasDefaultValue(true);

        builder.Property(u => u.IsDeleted)
            .HasDefaultValue(false);

        builder.Property(u => u.CreatedAt)
            .IsRequired();
        
        
        // Convert UserRole enum to underlying int for SQL storage
        builder.Property(u => u.RoleId)
            .HasConversion<int>()
            .IsRequired();

        // Bind relationship explicitly
        builder.HasOne(u => u.Role)
            .WithMany(r => r.Users)
            .HasForeignKey(u => u.RoleId)
            .OnDelete(DeleteBehavior.Restrict);
        
        // One-to-Many Relationship with RefreshTokens
        builder.HasMany(u => u.RefreshTokens)
            .WithOne(rt => rt.User)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}