using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Content.Domain.Entities;
using Content.Domain.ValueObjects;

namespace Content.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for QuestionOption entity
/// </summary>
public class QuestionOptionConfiguration : IEntityTypeConfiguration<QuestionOption>
{
    public void Configure(EntityTypeBuilder<QuestionOption> builder)
    {
        builder.ToTable("QuestionOptions");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(o => o.OptionText)
            .IsRequired()
            .HasMaxLength(4000)
            .HasConversion(
                v => v.Value,
                v => OptionText.Create(v));

        builder.Property(o => o.QuestionId)
            .HasColumnName("QuestionID")
            .IsRequired();

        builder.Property(o => o.IsCorrect)
            .IsRequired();

        builder.Property(o => o.CreatedDate)
            .IsRequired();

        builder.Property(o => o.CreatedBy)
            .HasMaxLength(100);

        builder.Property(o => o.ModifiedDate);

        builder.Property(o => o.ModifiedBy)
            .HasMaxLength(100);

        // Configure relationship with Question
        builder.HasOne(o => o.Question)
            .WithMany(q => q.Options)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF Core
        builder.Ignore(o => o.DomainEvents);
    }
}
