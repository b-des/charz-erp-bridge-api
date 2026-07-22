using CharzPiexApi.Data;
using CharzPiexApi.Domain;
using FastEndpoints;

namespace CharzPiexApi.Endpoints;

public record DefectPart(string Value, string Label, int Quantity, int Replace, int Repair, int Missing);

public record DefectRequest(
    string Model,
    string OrderNumber,
    string ChassisNumber,
    string EngineNumber,
    List<DefectPart> SelectedItems);

public record DefectResponse(string? DocumentRef);

public class PostDefectEndpoint(OneCService oneCService, AppDbContext db) : Endpoint<DefectRequest, DefectResponse>
{
    public override void Configure()
    {
        Post("/api/defect");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DefectRequest request, CancellationToken ct)
    {
        var result = await oneCService.SendDefect(request, ct);
        await Send.OkAsync(new DefectResponse(result), ct);
    }
}