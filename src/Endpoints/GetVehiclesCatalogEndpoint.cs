using CharzPiexApi.Domain;
using FastEndpoints;

namespace CharzPiexApi.Endpoints;

public class GetVehiclesCatalogEndpoint(ICatalogService catalogService) : EndpointWithoutRequest<List<VehicleCatalog>>
{
    public override void Configure()
    {
        Get("/api/vehicles");
        AllowAnonymous();
        Description(x =>
        {
            x.WithName("GetVehicles");
            x.WithTags("Vehicles");
        });
        Summary(s =>
        {
            s.Summary = "Gets vehicles catalog";
            s.Description = "Returns list of available vehicles";
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(await catalogService.GetCatalogs(), ct);
    }
}