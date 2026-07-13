using CharzPiexApi.Domain;
using CharzPiexApi.Endpoints;

namespace CharzPiexApi;

public class OneCService(IOneCClient oneCClient, FileService fileService) : IOneCService
{
    public async Task<string> SendDefect(DefectRequest request, CancellationToken ct)
    {
        var dateTime = DateTime.Now;
        var filename = $"defect-{dateTime.ToShortDateString()}-{dateTime.ToShortTimeString().Replace(":", ".")}.json";
        var path = await fileService.Write(filename, request.Dump());
        var result = oneCClient.Eval<string>(OneCMethods.Defect, $"{path}");
        return result;
    }
}