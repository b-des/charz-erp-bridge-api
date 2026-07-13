using System.Text;
using CharzPiexApi;
using CharzPiexApi.Data;
using CharzPiexApi.Domain;
using FastEndpoints;
using FastEndpoints.Security;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();
Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod());
});
var jwtSettings = builder.Configuration.GetSection(Constants.Jwt);
builder.Services.AddAuthenticationJwtBearer(s => s.SigningKey = jwtSettings[Constants.Key]!);
builder.Services.AddAuthorization();
builder.Configuration.AddEnvironmentVariables();
builder.Services.AddFastEndpoints();
// Add services to the container.
builder.Services.AddSingleton<IOneCClient, OneCClient>();
builder.Services.AddSingleton<ICatalogService, CatalogService>();
builder.Services.AddScoped<FileService>();
builder.Services.AddScoped<OneCService>();
// SqLite configuration
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' was not found.");
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlite(connectionString));

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}
app.UseCors("Frontend");
app.UseAuthentication();
app.UseAuthorization();
app.UseFastEndpoints();
app.Run();