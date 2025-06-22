using Microsoft.EntityFrameworkCore;
using Wisgenix.Data.Entities;
using Wisgenix.Data.Configurations;

namespace Wisgenix.Data;

public class AppDbContext: DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAssessment> UserAssessments { get; set; }
    public DbSet<UserAssessmentQuestion> UserAssessmentQuestions { get; set; }
    public DbSet<Question> Questions { get; set; }
    public DbSet<QuestionOption> QuestionOptions { get; set; }
    public DbSet<UserResponse> UserResponses { get; set; }
    public DbSet<Topic> Topics { get; set; }
    public DbSet<Subject> Subjects { get; set; }
    public DbSet<LearningRecommendation> LearningRecommendations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply entity configurations
        modelBuilder.ApplyConfiguration(new SubjectConfiguration());
        modelBuilder.ApplyConfiguration(new TopicConfiguration());
        modelBuilder.ApplyConfiguration(new QuestionConfiguration());
    }

    /// <summary>
    /// Only for SQLite database. If the database does not exist, it will be created.
    /// </summary>
    public void EnsureDatabaseCreated()
    {
        Database.EnsureCreated();
    }
}
