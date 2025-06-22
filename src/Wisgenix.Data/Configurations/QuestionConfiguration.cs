using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wisgenix.Data.Entities;

namespace Wisgenix.Data.Configurations
{
    public class QuestionConfiguration : IEntityTypeConfiguration<Question>
    {
        public void Configure(EntityTypeBuilder<Question> builder)
        {
            builder.HasIndex(q => new { q.QuestionText, q.TopicID }).IsUnique();
            
            builder.HasOne(q => q.Topic)
                .WithMany(t => t.Questions)
                .HasForeignKey(q => q.TopicID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
