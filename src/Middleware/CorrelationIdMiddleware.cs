using System.Diagnostics;
using Serilog.Context;

namespace CharzPiexApi.Middleware;

public class CorrelationIdMiddleware(
    RequestDelegate next,
    ILogger<CorrelationIdMiddleware> logger)
{
    private const string HeaderName = "X-Correlation-ID";

    public async Task InvokeAsync(HttpContext context)
    {
        // Якщо клієнт вже передав Correlation ID - використовуємо його
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var value)
            ? value.ToString()
            : Guid.NewGuid().ToString("N");

        // Зберігаємо в HttpContext для інших сервісів
        context.Items["CorrelationId"] = correlationId;

        // Повертаємо клієнту
        context.Response.Headers[HeaderName] = correlationId;

        var stopwatch = Stopwatch.StartNew();

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            logger.LogInformation(
                "HTTP {Method} {Path} started",
                context.Request.Method,
                context.Request.Path);

            try
            {
                await next(context);

                stopwatch.Stop();

                logger.LogInformation(
                    "HTTP {Method} {Path} finished {StatusCode} in {Elapsed} ms",
                    context.Request.Method,
                    context.Request.Path,
                    context.Response.StatusCode,
                    stopwatch.ElapsedMilliseconds);
            }
            catch (Exception ex)
            {
                stopwatch.Stop();

                logger.LogError(
                    ex,
                    "HTTP {Method} {Path} failed after {Elapsed} ms",
                    context.Request.Method,
                    context.Request.Path,
                    stopwatch.ElapsedMilliseconds);

                throw;
            }
        }
    }
}