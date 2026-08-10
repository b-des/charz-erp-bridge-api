using CharzPiexApi.Domain;
using FastEndpoints;

namespace CharzPiexApi.Endpoints;

public class HealthEndpoint(IOneCClient oneCClient) : EndpointWithoutRequest<HealthStatus>
{
    public override void Configure()
    {
        Get("/api/health");
        AllowAnonymous();
        Description(x =>
        {
            x.WithName("GetHealth");
            x.WithTags("Health");
        });
        Summary(s =>
        {
            s.Summary = "Health Check";
            s.Description = "Checks connection to 1C.";

            s.Response<HealthStatus>(
                200,
                "Connection established");

            s.Response(
                500,
                "1C unavailable");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(oneCClient.CheckHealth(), ct);
    }
}