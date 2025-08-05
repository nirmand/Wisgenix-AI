using Microsoft.EntityFrameworkCore;
using MediatR;
using Serilog;
using Serilog.Enrichers.CorrelationId;
using Content.Infrastructure.Data;
using Content.Domain.Repositories;
using Content.Infrastructure.Repositories;
using Content.Application.Mappings;
using Content.Application.Validators;
using Content.Application.Commands;
using Content.Application.Handlers;
using Content.API.Middleware;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog with configurable path and correlation IDs
var logsPath = builder.Configuration["LogsPath"] ?? "../../../logs";
var databaseProvider = builder.Configuration["DatabaseProvider"] ?? "";
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var fullLogsPath = Path.GetFullPath(logsPath);

if (String.IsNullOrEmpty(databaseProvider))
{
    throw new Exception("Missing Configurations - Database connnection provider not found!");
}
if (String.IsNullOrEmpty(connectionString))
{
    throw new Exception("Missing Configurations - Connection String not found!");
}

// Ensure logs directory exists
if (!Directory.Exists(fullLogsPath))
{
    Directory.CreateDirectory(fullLogsPath);
}

Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithCorrelationId()
    .Enrich.FromLogContext()
    .Enrich.WithEnvironmentName()
    .Enrich.WithThreadId()
    .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] [{CorrelationId}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .WriteTo.File(
        path: Path.Combine(fullLogsPath, "wisgenix-.txt"),
        rollingInterval: RollingInterval.Day,
        retainedFileCountLimit: 7,
        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} {Level:u3}] [{CorrelationId}] {Message:lj} {Properties:j}{NewLine}{Exception}")
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Content API", Version = "v1" });
});

// Add correlation ID services
builder.Services.AddHttpContextAccessor();

// Add CORS
var corsOrigins = builder.Configuration.GetSection("CORS:Origins").Get<string[]>() ?? new[] { "http://localhost:3000" };
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(corsOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add DbContext
if (databaseProvider == "SqlServer")
{
    builder.Services.AddDbContext<ContentDbContext>(options =>
        options.UseSqlServer(connectionString));
}
else
{
    builder.Services.AddDbContext<ContentDbContext>(options =>
        options.UseSqlite(connectionString));
}
// Add Unit of Work
    builder.Services.AddScoped<IUnitOfWork>(provider => provider.GetRequiredService<ContentDbContext>());

// Add repositories
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();

// Add logging service
builder.Services.AddScoped<ILoggingService, SerilogLoggingService>();
builder.Services.AddSingleton(Log.Logger);

// Add MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(CreateSubjectCommand).Assembly);
    cfg.RegisterServicesFromAssembly(typeof(CreateSubjectCommandHandler).Assembly);
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(ContentMappingProfile));

// Add validators
builder.Services.AddScoped<IAddSubjectRequestValidator, AddSubjectRequestValidator>();
builder.Services.AddScoped<IUpdateSubjectRequestValidator, UpdateSubjectRequestValidator>();
builder.Services.AddScoped<IAddTopicRequestValidator, AddTopicRequestValidator>();
builder.Services.AddScoped<IUpdateTopicRequestValidator, UpdateTopicRequestValidator>();
builder.Services.AddScoped<IAddQuestionRequestValidator, AddQuestionRequestValidator>();
builder.Services.AddScoped<IUpdateQuestionRequestValidator, UpdateQuestionRequestValidator>();
builder.Services.AddScoped<IAddQuestionOptionRequestValidator, AddQuestionOptionRequestValidator>();
builder.Services.AddScoped<IUpdateQuestionOptionRequestValidator, UpdateQuestionOptionRequestValidator>();

var app = builder.Build();

// Configure the HTTP request pipeline
// if (app.Environment.IsDevelopment()) //TODO: Uncomment this later on.
// {
    app.UseSwagger();
    app.UseSwaggerUI();
// }

// Add Serilog request logging middleware
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
    options.GetLevel = (httpContext, elapsed, ex) => ex != null
        ? Serilog.Events.LogEventLevel.Error
        : elapsed > 1000
            ? Serilog.Events.LogEventLevel.Warning
            : Serilog.Events.LogEventLevel.Information;
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault() ?? "Unknown");
        diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown");
    };
});

// Add global exception handling middleware
app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();

// Ensure database directory and database are created (skip for testing environment)
var skipDbInit = Environment.GetEnvironmentVariable("SKIP_DB_INIT") == "true" ||
                 app.Environment.IsEnvironment("Testing");

if (!skipDbInit)
{
    try
    {
        using (var scope = app.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();

            // Extract the database file path from connection string
            if (connectionString != null && connectionString.Contains("Data Source="))
            {
                var dataSourceStart = connectionString.IndexOf("Data Source=") + "Data Source=".Length;
                var dataSourceEnd = connectionString.IndexOf(';', dataSourceStart);
                if (dataSourceEnd == -1) dataSourceEnd = connectionString.Length;

                var dbPath = connectionString[dataSourceStart..dataSourceEnd].Trim();
                var dbDirectory = Path.GetDirectoryName(Path.GetFullPath(dbPath));

                // Create directory if it doesn't exist
                if (!string.IsNullOrEmpty(dbDirectory) && !Directory.Exists(dbDirectory))
                {
                    Directory.CreateDirectory(dbDirectory);
                    Console.WriteLine($"Created database directory: {dbDirectory}");
                }
            }

            context.Database.EnsureCreated();
            Console.WriteLine($"Database initialized successfully.");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Database initialization skipped due to error: {ex.Message}");
    }
}

app.Run();

// Make the Program class accessible for testing
public partial class Program { }
