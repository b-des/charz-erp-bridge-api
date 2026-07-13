using CharzPiexApi.Domain;
using FastEndpoints;

namespace CharzPiexApi.Endpoints;

public class HealthEndpoint(IOneCClient oneCClient) : EndpointWithoutRequest<HealthStatus>
{
    public override void Configure()
    {
        Get("/api/health");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(oneCClient.CheckHealth(), ct);
    }
}