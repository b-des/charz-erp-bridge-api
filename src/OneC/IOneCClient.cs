using CharzPiexApi.Domain;

namespace CharzPiexApi;

public interface IOneCClient
{
    void Initialize();
    HealthStatus CheckHealth();
    T Eval<T>(string method, params string[]? args);
}