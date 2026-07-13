using System.Text.Json;
using CharzPiexApi.Domain;
using CharzPiexApi.Utils;

namespace CharzPiexApi;

public class CatalogService(IWebHostEnvironment environment, IConfiguration config, ILogger<CatalogService> logger) : ICatalogService
{
    public async Task<List<VehicleCatalog>> GetCatalogs()
    {
        var catalogsPath = config.GetSection(Constants.File);
        var path = Path.Combine(environment.ContentRootPath, catalogsPath[Constants.CatalogsPath]!);
        logger.LogInformation($"Loading catalogs from {path}");
        return  Directory
            .EnumerateFiles(path, "*.json")
            .Select(Path.GetFileNameWithoutExtension)
            .Select(x =>
            {
                var json = File.ReadAllText(Path.Combine(path, x + ".json"));

                var jsonOptions = new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                };
                var nodes = JsonSerializer.Deserialize<List<object>>(json, jsonOptions);
                return new VehicleCatalog(x!, VehicleUtils.GetReadableName(x!), nodes);
            })
            .ToList();
    }
}