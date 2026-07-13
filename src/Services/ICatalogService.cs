using CharzPiexApi.Domain;

namespace CharzPiexApi;

public interface ICatalogService
{
    Task<List<VehicleCatalog>> GetCatalogs();

}