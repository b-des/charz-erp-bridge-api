using CharzPiexApi.Data;
using CharzPiexApi.Domain;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace CharzPiexApi.Endpoints;

public class GetDefectsListEndpoint(AppDbContext db) : EndpointWithoutRequest<List<DefectEntity>>
{
    public override void Configure()
    {
        Get("/api/defect");
        AllowAnonymous();
        Description(x =>
        {
            x.WithName("GetDefects");
            x.WithTags("Defect");
        });
        Summary(s =>
        {
            s.Summary = "Gets Defects";
            s.Description = "Returns list of all sent Defects";
        });
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