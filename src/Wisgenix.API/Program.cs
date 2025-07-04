using Wisgenix.API;
using Wisgenix.API.Extensions;
using Wisgenix.API.Middleware;
using Wisgenix.Core.Logger;
using Wisgenix.Data;
using Wisgenix.DTO.Mappings;
using Azure.Monitor.OpenTelemetry.Exporter;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

// Configure SeriLog
var logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
    .Enrich.FromLogContext();

if (builder.Environment.IsDevelopment() || builder.Environment.IsEnvironment("localhost"))
{
    // Retrieve root folder path
    var rootFolderPath = System.IO.Directory.GetParent(System.IO.Directory.GetParent(System.Environment.CurrentDirectory).ToString()).ToString();

    var logPath = Path.Combine(rootFolderPath, builder.Configuration["LocalLogs:LogFilePath"]);

    // Ensure directory exists
    Directory.CreateDirectory(Path.GetDirectoryName(logPath));

    logger.WriteTo.Console(
        outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}")
          .WriteTo.File(logPath,
          rollingInterval: RollingInterval.Day,
          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff} [{Level:u3}] [{CorrelationId}] {Message:lj}{NewLine}{Exception}");
}
else
{
    logger.WriteTo.ApplicationInsights(builder.Configuration["ApplicationInsights:ConnectionString"], TelemetryConverter.Traces);
}

Log.Logger = logger.CreateLogger();
builder.Host.UseSerilog();
builder.Services.AddSingleton(Log.Logger);

// Configure OpenTelemetry once with conditional configuration
builder.Services.AddOpenTelemetry()
    .ConfigureResource(resource =>
        resource.AddService(serviceName: "Wisgenix.API"))
    .WithTracing(tracing =>
    {
        tracing.AddAspNetCoreInstrumentation()
              .AddHttpClientInstrumentation();

        // Add Azure Monitor only for non-development environments
        if (!builder.Environment.IsDevelopment() && !builder.Environment.IsEnvironment("localhost"))
        {
            tracing.AddAzureMonitorTraceExporter(options =>
            {
                options.ConnectionString =
                    builder.Configuration["ApplicationInsights:ConnectionString"];
            });
        }
    });

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
        Title = "WisGenix API",
        Version = "v1",
        Description = "API documentation for WisGenix."
    });
});

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

builder.Services.InjectRepositories();
builder.Services.RegisterDTOValidators();

// Register AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(TopicMappingProfile).Assembly));

builder.Services.AddControllers();

var allowedOrigins = builder.Configuration.GetSection("CORS:Origins").Get<string[]>();

// Add CORS services
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost-UI",
        builder =>
        {
            if (allowedOrigins != null && allowedOrigins.Length > 0)
            {
                builder.WithOrigins(allowedOrigins) // Pass the array of origins
                       .AllowAnyMethod()
                       .AllowAnyHeader()
                       .AllowCredentials();
            }            
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("localhost"))
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
}
else
{
    app.UseHttpsRedirection();
}

app.UseRouting();
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseSerilogRequestLogging(opts =>
{
    opts.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
        {
            diagnosticContext.Set("CorrelationId", correlationId?.ToString());
        }
    };
});
app.UseCors("AllowLocalhost-UI");
app.UseAuthorization();
app.MapControllers();

app.Run();