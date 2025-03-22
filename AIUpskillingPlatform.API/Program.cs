using AIUpskillingPlatform.API;
using AIUpskillingPlatform.Data;
using AIUpskillingPlatform.Repositories;
using AIUpskillingPlatform.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

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