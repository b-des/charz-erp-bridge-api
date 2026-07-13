using CharzPiexApi.Domain;
using Microsoft.EntityFrameworkCore;

namespace CharzPiexApi.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<DefectEntity> DefectEntityItems => Set<DefectEntity>();
}