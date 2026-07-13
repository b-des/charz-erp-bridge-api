using CharzPiexApi.Endpoints;

namespace CharzPiexApi;

public interface IOneCService
{
    Task<string> SendDefect(DefectRequest request, CancellationToken ct);
}