using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wisgenix.Data.Entities;

namespace Wisgenix.Data.Configurations
{
    public class TopicConfiguration : IEntityTypeConfiguration<Topic>
    {
        public void Configure(EntityTypeBuilder<Topic> builder)
        {
            builder.HasIndex(t => new { t.TopicName, t.SubjectID }).IsUnique();
            
            builder.HasOne(t => t.Subject)
                .WithMany(s => s.Topics)
                .HasForeignKey(t => t.SubjectID)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
