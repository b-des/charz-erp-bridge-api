namespace CharzPiexApi.Domain;

public record HealthStatus(
    bool Healthy,
    string Message);