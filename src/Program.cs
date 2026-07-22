using System.Text;
using CharzPiexApi;
using CharzPiexApi.Data;
using CharzPiexApi.Domain;
using CharzPiexApi.Middleware;
using CharzPiexApi.Utils;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;
using Serilog;

DotNetEnv.Env.Load();
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

// --------------------
// Serilog
// --------------------
Logger.RegisterLogger();

var builder = WebApplication.CreateBuilder(args);

// Використовувати Serilog потрібно ДО Build()
builder.Host.UseSerilog();

builder.Configuration.AddEnvironmentVariables();

// --------------------
// CORS
// --------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});

// --------------------
// Authentication
// --------------------
var jwtSettings = builder.Configuration.GetSection(Constants.Jwt);

builder.Services.AddAuthenticationJwtBearer(s =>
    s.SigningKey = jwtSettings[Constants.Key]!);

builder.Services.AddAuthorization();

// --------------------
// FastEndpoints
// --------------------
builder.Services.AddFastEndpoints();

// --------------------
// Dependency Injection
// --------------------
builder.Services.AddSingleton<IOneCClient, OneCClient>();
builder.Services.AddSingleton<ICatalogService, CatalogService>();

builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<OneCService>();

// --------------------
// Database
// --------------------
var connectionString =
    builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException(
        "Connection string 'DefaultConnection' was not found.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(connectionString));



try
{
    Log.Information("Starting CharzPiexApi");

    // --------------------
    // Build
    // --------------------
    var app = builder.Build();
    
    // --------------------
    // Middlewares
    // --------------------
    app.UseCorrelationId();

    // --------------------
    // Migrations
    // --------------------
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }

    // --------------------
    // Middleware
    // --------------------
    app.UseCors("Frontend");

    app.UseAuthentication();
    app.UseAuthorization();

    app.UseFastEndpoints();

    app.Run();
}
catch (Exception ex) when (ex.GetType().Name is "HostAbortedException" or "StopTheHostException")
{
    // Let EF Core handle its intentional design-time shutdowns
    throw; 
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}