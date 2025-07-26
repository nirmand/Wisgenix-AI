using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Content.Domain.Entities;

namespace Content.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Subject entity
/// </summary>
public class SubjectConfiguration : IEntityTypeConfiguration<Subject>
{
    public void Configure(EntityTypeBuilder<Subject> builder)
    {
        builder.ToTable("Subjects");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(s => s.SubjectName)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(s => s.CreatedDate)
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasMaxLength(100);

        builder.Property(s => s.ModifiedDate);

        builder.Property(s => s.ModifiedBy)
            .HasMaxLength(100);

        // Unique constraint on SubjectName
        builder.HasIndex(s => s.SubjectName)
            .IsUnique();

        // Configure relationship with Topics
        builder.HasMany(s => s.Topics)
            .WithOne(t => t.Subject)
            .HasForeignKey(t => t.SubjectId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF Core
        builder.Ignore(s => s.DomainEvents);
    }
}
