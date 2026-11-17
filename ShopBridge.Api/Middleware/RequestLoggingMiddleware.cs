using System.Diagnostics;
using ShopBridge.Application.Abstractions.Logging;

namespace ShopBridge.Api.Middleware;

public class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> logger)
{
    public async Task InvokeAsync(
        HttpContext context,
        ILogService logService)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalBody = context.Response.Body;

        await using var buffer = new MemoryStream();
        context.Response.Body = buffer;

        try
        {
            await next(context);
            stopwatch.Stop();

            if (context.Response.StatusCode < 400)
            {
                await TryLogAsync(context, logService, buffer, stopwatch.Elapsed);
            }

            buffer.Position = 0;
            await buffer.CopyToAsync(originalBody, context.RequestAborted);
        }
        finally
        {
            context.Response.Body = originalBody;
        }
    }

    private async Task TryLogAsync(
        HttpContext context,
        ILogService logService,
        MemoryStream buffer,
        TimeSpan elapsed)
    {
        try
        {
            var request = new
            {
                Method = context.Request.Method,
                Path = context.Request.Path.Value,
                QueryString = context.Request.QueryString.Value
            };

            var response = new
            {
                StatusCode = context.Response.StatusCode,
                Body = await ReadJsonBodyAsync(
                    context.Response, buffer, context.RequestAborted)
            };

            await logService.LogApplicationAsync(
                request, response, elapsed, context.RequestAborted);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to persist application log.");
        }
    }

    private static async Task<string?> ReadJsonBodyAsync(
        HttpResponse response,
        MemoryStream buffer,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(response.ContentType) ||
            !response.ContentType.Contains(
                "json", StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        buffer.Position = 0;
        using var reader = new StreamReader(buffer, leaveOpen: true);
        var body = await reader.ReadToEndAsync(cancellationToken);
        buffer.Position = 0;

        return body;
    }
}