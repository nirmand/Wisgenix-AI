using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Content.Domain.Entities;
using Content.Domain.ValueObjects;

namespace Content.Infrastructure.Data.Configurations;

/// <summary>
/// Entity Framework configuration for Question entity
/// </summary>
public class QuestionConfiguration : IEntityTypeConfiguration<Question>
{
    public void Configure(EntityTypeBuilder<Question> builder)
    {
        builder.ToTable("Questions");

        builder.HasKey(q => q.Id);

        builder.Property(q => q.Id)
            .HasColumnName("ID")
            .ValueGeneratedOnAdd();

        builder.Property(q => q.QuestionText)
            .IsRequired()
            .HasMaxLength(1000)
            .HasConversion(
                v => v.Value,
                v => QuestionText.Create(v));

        builder.Property(q => q.TopicId)
            .HasColumnName("TopicID")
            .IsRequired();

        builder.Property(q => q.DifficultyLevel)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => DifficultyLevel.Create(v));

        builder.Property(q => q.MaxScore)
            .IsRequired()
            .HasConversion(
                v => v.Value,
                v => MaxScore.Create(v));

        builder.Property(q => q.GeneratedBy)
            .IsRequired()
            .HasConversion<int>();

        builder.Property(q => q.QuestionSourceReference)
            .HasMaxLength(500);

        builder.Property(q => q.CreatedDate)
            .IsRequired();

        builder.Property(q => q.CreatedBy)
            .HasMaxLength(100);

        builder.Property(q => q.ModifiedDate);

        builder.Property(q => q.ModifiedBy)
            .HasMaxLength(100);

        // Unique constraint on QuestionText and TopicId combination
        builder.HasIndex(q => new { q.QuestionText, q.TopicId })
            .IsUnique();

        // Configure relationship with Topic
        builder.HasOne(q => q.Topic)
            .WithMany(t => t.Questions)
            .HasForeignKey(q => q.TopicId)
            .OnDelete(DeleteBehavior.Cascade);

        // Configure relationship with QuestionOptions
        builder.HasMany(q => q.Options)
            .WithOne(o => o.Question)
            .HasForeignKey(o => o.QuestionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore domain events for EF Core
        builder.Ignore(q => q.DomainEvents);
    }
}
