using CharzPiexApi.Domain;
using FastEndpoints;

namespace CharzPiexApi.Endpoints;

public class GetVehiclesCatalogEndpoint(ICatalogService catalogService) : EndpointWithoutRequest<List<VehicleCatalog>>
{
    public override void Configure()
    {
        Get("/api/vehicles");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync( await catalogService.GetCatalogs(), ct);
    }
}