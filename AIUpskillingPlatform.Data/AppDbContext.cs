using Microsoft.EntityFrameworkCore;
using AIUpskillingPlatform.Data.Entities;
namespace AIUpskillingPlatform.Data;

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
    public DbSet<LearningRecommendation> LearningRecommendations { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Add model configurations here if needed
    }

    /// <summary>
    /// Only for SQLite database. If the database does not exist, it will be created.
    /// </summary>
    public void EnsureDatabaseCreated()
    {
        Database.EnsureCreated();
    }
}
