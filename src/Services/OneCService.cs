using CharzPiexApi.Data;
using CharzPiexApi.Domain;
using CharzPiexApi.Endpoints;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace CharzPiexApi;

public class OneCService(
    IOneCClient oneCClient,
    FileService fileService,
    AppDbContext db,
    ILogger<PostDefectEndpoint> logger) : IOneCService
{
    public async Task<string?> SendDefect(DefectRequest request, CancellationToken ct)
    {
        logger.LogInformation("Sending Defect: {@request}", request);
        var dateTime = DateTime.Now;
        var filename = $"defect-{dateTime.ToShortDateString()}-{dateTime.ToShortTimeString().Replace(":", ".")}.json";
        var path = await fileService.Write(filename, request.Dump());
        var saved = await db.DefectEntityItems.FirstOrDefaultAsync(
            d => d.Model == request.Model && d.Order == request.OrderNumber && d.Chassis == request.ChassisNumber &&
                 d.Engine == request.EngineNumber,
            ct);
        logger.LogInformation("Found Defect: {@saved}", saved);
        var result = oneCClient.Eval<string?>(OneCMethods.Defect, $"{path}", $"{saved?.DocumentRef}");
        logger.LogInformation("Result from 1C defect function: {@result}", result);
        if (result.IsNullOrEmpty())
        {
            return null;
        }
        var toSave = new DefectEntity
        {
            Model = request.Model,
            Order = request.OrderNumber,
            Chassis = request.ChassisNumber,
            Engine = request.EngineNumber,
            DocumentRef = result,
            FilePath = path
        };
        if (saved == null)
        {
            logger.LogInformation("Saving Defect: {@toSave}", toSave);
            db.DefectEntityItems.Add(toSave);
        }
        else
        {
            saved.Model = request.Model;
            saved.Order = request.OrderNumber;
            saved.Chassis = request.ChassisNumber;
            saved.Engine = request.EngineNumber;
            saved.DocumentRef = result;
            saved.FilePath = path;
            logger.LogInformation("Updating Defect: {@saved}", saved);
        }

        await db.SaveChangesAsync(ct);

        return result;
    }
}