using AIUpskillingPlatform.API;
using AIUpskillingPlatform.Core.Logger;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Repositories;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Serilog.Events;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Azure.Monitor.OpenTelemetry.Exporter;

var builder = WebApplication.CreateBuilder(args);

// Configure SeriLog
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext();

if (builder.Environment.IsDevelopment())
{
    // Retrieve root folder path
    var rootFolderPath = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.Environment.CurrentDirectory).ToString()).ToString(); 

    var logPath = Path.Combine(rootFolderPath, builder.Configuration["LocalLogs:LogFilePath"]);
    
    // Ensure directory exists
    Directory.CreateDirectory(Path.GetDirectoryName(logPath));
    
    logger.WriteTo.Console()
          .WriteTo.File(logPath,
          rollingInterval: RollingInterval.Day,
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] {Message:lj}{NewLine}{Exception}");
}
else
{
    logger.WriteTo.ApplicationInsights(builder.Configuration["ApplicationInsights:ConnectionString"], TelemetryConverter.Traces);
}

Log.Logger = logger.CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddSingleton(Log.Logger);

// Configure OpenTelemetry
builder.Services.AddOpenTelemetry()
.ConfigureResource(resource => resource.AddService(serviceName: "AIUpskillingPlatform.API"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation());

// For QA and Production, add Azure Monitor
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource => resource.AddService(serviceName: "AIUpskillingPlatform.API"))
    .WithTracing(tracing => tracing
        .AddAspNetCoreInstrumentation()
        .AddHttpClientInstrumentation()
        // Add Azure Monitor Exporter if not in development
        .AddAzureMonitorTraceExporter(options =>
        {
            options.ConnectionString = builder.Configuration["ApplicationInsights:ConnectionString"];
        }));
}
// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry();

// Add Logger
builder.Services.AddScoped<ILoggingService, SerilogLoggingService>();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "AIUpsklillingPlatform API",
        Version = "v1",
        Description = "API documentation for AIUpskillingPlatform."
    });
});

// Add Repository
builder.Services.AddScoped<ITopicRepository, TopicRepository>();
builder.Services.AddScoped<IQuestionRepository, QuestionRepository>();
builder.Services.AddScoped<IQuestionOptionRepository, QuestionOptionRepository>();
builder.Services.AddScoped<ISubjectRepository, SubjectRepository>();

// Add Controllers
builder.Services.AddControllers();

if (builder.Configuration.GetValue<int>("DatabaseType") == (int)DatabaseType.Sqlite)
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlite(builder.Configuration.GetConnectionString("SQLite")));
}
else
{
    builder.Services.AddDbContext<AppDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("AzureSQL")));
}

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        dbContext.EnsureDatabaseCreated(); // Creates DB if it does not exist
    }

    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "AIUpsklilling Platform V1");
        c.RoutePrefix = string.Empty;// Serve the Swagger UI at the app's root
    });

    app.MapOpenApi();
}


app.UseHttpsRedirection();

// Add routing
app.UseRouting();
app.UseAuthorization();
app.MapControllers();

app.Run();