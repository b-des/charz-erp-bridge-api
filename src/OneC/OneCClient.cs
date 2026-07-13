using CharzPiexApi.Domain;

namespace CharzPiexApi;

public class OneCClient(IConfiguration config, ILogger<OneCClient> logger) : IOneCClient, IDisposable
{
    private dynamic? _app;
    private readonly object _lock = new();
    private readonly object _sync = new();


    public void Initialize()
    {
        lock (_lock)
        {
            if (_app != null)
                return;

            var oneCConfig = config.GetSection(Constants.OneCPrefix);
            logger.LogInformation("1C COM server initializing");
            var type = Type.GetTypeFromProgID(oneCConfig[Constants.ProgId]) ??
                       throw new Exception("1C COM server not found");

            _app = Activator.CreateInstance(type);

            var cmd = $"/D\"{oneCConfig[Constants.DbDir]}\" /M /N{oneCConfig[Constants.User]}";
            bool result = _app.Initialize(_app.RMTrade, cmd, Constants.NoSplashShow);
            logger.LogInformation($"1C COM server initialized: {result}");
        }
    }

    public HealthStatus CheckHealth()
    {
        lock (_sync)
        {
            try
            {
                logger.LogInformation("1C COM server checking");
                Initialize();
                var result = _app?.EvalExpr($"{OneCMethods.CurrentDate}()");
                logger.LogInformation($"1C COM server check completed: {result}");                                                                                                                                                                                                                                                                                                                                                                                     
                if (result != null)
                    return new(true, "Connected");

                return new(false, $"Unexpected response: {result}");
            }
            catch (Exception ex)
            {
                return new(false, ex.Message);
            }
        }
    }

    public T Eval<T>(string method, params string[]? args)
    {
        lock (_sync)
        {
            Initialize();
            var expression = args == null || args.Length == 0
                ? $"{method}()"
                : $"{method}(\"{string.Join(",", args)}\")";
            logger.LogInformation("Sending 1C request: {expression}", expression);
            try
            {
                var result = _app?.EvalExpr(expression);
                return (T)Convert.ChangeType(result, typeof(T));
            }
            catch (Exception e)
            {
                logger.LogError(e, "1C COM server error");
            }

            return default;
        }
    }

    public void Dispose()
    {
        if (_app == null) return;
        try
        {
            _app.Quit(); // якщо метод доступний
            logger.LogInformation("1C COM server disposed");
        }
        catch (Exception e)
        {
            // Ігноруємо, якщо COM не підтримує Quit
            Console.WriteLine(e);
        }
        finally
        {
            System.Runtime.InteropServices.Marshal.FinalReleaseComObject(_app);

            _app = null;
        }
    }
}