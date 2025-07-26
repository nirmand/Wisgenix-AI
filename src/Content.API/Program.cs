using Microsoft.EntityFrameworkCore;
using MediatR;
using Serilog;
using Content.Infrastructure.Data;
using Content.Domain.Repositories;
using Content.Infrastructure.Repositories;
using Content.Application.Mappings;
using Content.Application.Validators;
using Content.Application.Commands;
using Content.Application.Handlers;
using Wisgenix.SharedKernel.Application;
using Wisgenix.SharedKernel.Infrastructure.Logging;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.Console()
    .WriteTo.File("logs/content-api-.log", rollingInterval: RollingInterval.Day)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Content API", Version = "v1" });
});

// Add DbContext
builder.Services.AddDbContext<ContentDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

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
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ContentDbContext>();
    context.Database.EnsureCreated();
}

app.Run();

// Make the Program class accessible for testing
public partial class Program { }
