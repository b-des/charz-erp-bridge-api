using CharzPiexApi.Data;
using CharzPiexApi.Domain;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace CharzPiexApi.Endpoints;



public class GetDefectsListEndpoint(OneCService oneCService, AppDbContext db) : EndpointWithoutRequest<List<DefectEntity>>
{
    public override void Configure()
    {
        Get("/api/defect");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var items = await db.DefectEntityItems
            .AsNoTracking()
            .OrderBy(x => x.Id)
            .ToListAsync(ct);

        await Send.OkAsync(items, ct);
    }
}