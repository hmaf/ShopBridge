using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using ShopBridge.Application.Abstractions.Logging;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Domain.Exceptions;

namespace ShopBridge.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, message) = Map(exception);

        try
        {
            // IExceptionHandler is a singleton, ILogService is scoped.
            var logService = httpContext.RequestServices
                .GetRequiredService<ILogService>();

            var request = new
            {
                Method = httpContext.Request.Method,
                Path = httpContext.Request.Path.Value,
                QueryString = httpContext.Request.QueryString.Value,
                Body = await ReadRequestBodyAsync(
                    httpContext.Request, cancellationToken)
            };

            await LogAsync(
                logService, exception, request, cancellationToken);
        }
        catch (Exception loggingException)
        {
            logger.LogError(
                loggingException,
                "Failed to persist exception log.");
        }

        httpContext.Response.StatusCode = statusCode;

        await httpContext.Response.WriteAsJsonAsync(
            ApiResponse<object>.FailureResponse(message),
            cancellationToken);

        return true;
    }

    private static (int StatusCode, string Message) Map(Exception exception) =>
        exception switch
        {
            BusinessException => (
                StatusCodes.Status400BadRequest, exception.Message),

            ValidationException => (
                StatusCodes.Status400BadRequest, exception.Message),

            KeyNotFoundException => (
                StatusCodes.Status404NotFound, exception.Message),

            _ => (
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.")
        };

    private async Task LogAsync(
        ILogService logService,
        Exception exception,
        object request,
        CancellationToken cancellationToken)
    {
        switch (exception)
        {
            case BusinessException businessException:
                logger.LogWarning(
                    exception,
                    "A business error occurred. ErrorCode: {ErrorCode}",
                    businessException.ErrorCode);

                await logService.LogBusinessErrorAsync(
                    request,
                    businessException.ErrorCode,
                    businessException.Message,
                    cancellationToken);
                break;

            case ValidationException validationException:
                logger.LogWarning(exception, "Request validation failed.");

                await logService.LogBusinessErrorAsync(
                    request,
                    StatusCodes.Status400BadRequest,
                    validationException.Message,
                    cancellationToken);
                break;

            case KeyNotFoundException:
                logger.LogWarning(
                    exception, "Requested resource was not found.");

                await logService.LogBusinessErrorAsync(
                    request,
                    StatusCodes.Status404NotFound,
                    exception.Message,
                    cancellationToken);
                break;

            default:
                logger.LogError(
                    exception, "An unhandled system exception occurred.");

                await logService.LogSystemErrorAsync(
                    request,
                    exception.Message,
                    exception.StackTrace ?? string.Empty,
                    cancellationToken);
                break;
        }
    }

    private static async Task<string> ReadRequestBodyAsync(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.Body.CanSeek)
            return string.Empty;

        request.Body.Position = 0;

        using var reader = new StreamReader(request.Body, leaveOpen: true);
        var body = await reader.ReadToEndAsync(cancellationToken);

        request.Body.Position = 0;
        return body;
    }
}