
using FluentValidation;
using ShopBridge.Application.Abstractions.Logging;
using ShopBridge.Application.Common.Mapping;
using ShopBridge.Domain.Exceptions;
using System.Diagnostics;
using System.Runtime.ExceptionServices;

namespace ShopBridge.Api.Middleware;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(
        RequestDelegate next,
        ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(
        HttpContext context,
        ILogService logService)
    {
        var stopwatch = Stopwatch.StartNew();
        var originalResponseBody = context.Response.Body;

        await using var responseBuffer = new MemoryStream();

        Exception? exception = null;
        var shouldRethrow = false;

        context.Request.EnableBuffering();
        context.Response.Body = responseBuffer;

        try
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                exception = ex;
            }

            stopwatch.Stop();

            if (exception is null)
            {
                if (context.Response.StatusCode < 400)
                {
                    var request = new
                    {
                        Method = context.Request.Method,
                        Path = context.Request.Path.Value,
                        QueryString = context.Request.QueryString.Value
                    };

                    var responseBody =
                        await ReadJsonResponseBodyAsync(
                            context.Response,
                            responseBuffer,
                            context.RequestAborted);

                    var response = new
                    {
                        StatusCode = context.Response.StatusCode,
                        Body = responseBody
                    };

                    try
                    {
                        await logService.LogApplicationAsync(
                            request,
                            response,
                            stopwatch.Elapsed,
                            context.RequestAborted);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(
                            ex,
                            "Failed to persist application log.");
                    }
                }
            }
            else
            {
                var requestBody = await ReadRequestBodyAsync(
                    context.Request,
                    context.RequestAborted);

                var errorRequest = new
                {
                    Method = context.Request.Method,
                    Path = context.Request.Path.Value,
                    QueryString = context.Request.QueryString.Value,
                    Body = requestBody
                };

                var (statusCode, message) = exception switch
                {
                    BusinessException =>
                        (StatusCodes.Status400BadRequest, exception.Message),

                    ValidationException =>
                        (StatusCodes.Status400BadRequest, exception.Message),

                    KeyNotFoundException =>
                        (StatusCodes.Status404NotFound, exception.Message),

                    _ => (
                        StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred.")
                };

                try
                {
                    switch (exception)
                    {
                        case BusinessException businessException:
                            _logger.LogWarning(
                                exception,
                                "A business error occurred. ErrorCode: {ErrorCode}",
                                businessException.ErrorCode);

                            await logService.LogBusinessErrorAsync(
                                errorRequest,
                                businessException.ErrorCode,
                                businessException.Message,
                                context.RequestAborted);
                            break;

                        case ValidationException validationException:
                            _logger.LogWarning(
                                exception,
                                "Request validation failed.");

                            await logService.LogBusinessErrorAsync(
                                errorRequest,
                                400,
                                validationException.Message,
                                context.RequestAborted);
                            break;

                        case KeyNotFoundException:
                            _logger.LogWarning(
                                exception,
                                "Requested resource was not found.");

                            await logService.LogBusinessErrorAsync(
                                errorRequest,
                                404,
                                exception.Message,
                                context.RequestAborted);
                            break;

                        default:
                            _logger.LogError(
                                exception,
                                "An unhandled system exception occurred.");

                            await logService.LogSystemErrorAsync(
                                errorRequest,
                                exception.Message,
                                exception.StackTrace ?? string.Empty,
                                context.RequestAborted);
                            break;
                    }
                }
                catch (Exception loggingException)
                {
                    _logger.LogError(
                        loggingException,
                        "Failed to persist exception log.");
                }

                if (context.Response.HasStarted)
                {
                    _logger.LogWarning(
                        "The response has already started. Exception response cannot be written.");

                    shouldRethrow = true;
                }
                else
                {
                    responseBuffer.SetLength(0);

                    context.Response.StatusCode = statusCode;
                    context.Response.ContentType = "application/json";

                    var apiResponse =
                        ApiResponse<object>.FailureResponse(message);

                    await context.Response.WriteAsJsonAsync(apiResponse);
                }
            }

            // Copy the buffered response to the original response stream.
            responseBuffer.Position = 0;

            await responseBuffer.CopyToAsync(
                originalResponseBody,
                context.RequestAborted);

            if (shouldRethrow && exception is not null)
            {
                ExceptionDispatchInfo.Capture(exception).Throw();
            }
        }
        finally
        {
            context.Response.Body = originalResponseBody;
        }
    }

    private static async Task<string?> ReadJsonResponseBodyAsync(
        HttpResponse response,
        MemoryStream responseBuffer,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(response.ContentType) ||
            !response.ContentType.Contains(
                "json",
                StringComparison.OrdinalIgnoreCase))
        {
            return null;
        }

        responseBuffer.Position = 0;

        using var reader = new StreamReader(
            responseBuffer,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync(cancellationToken);

        responseBuffer.Position = 0;

        return body;
    }

    private static async Task<string> ReadRequestBodyAsync(
        HttpRequest request,
        CancellationToken cancellationToken)
    {
        if (!request.Body.CanSeek)
            return string.Empty;

        request.Body.Position = 0;

        using var reader = new StreamReader(
            request.Body,
            leaveOpen: true);

        var body = await reader.ReadToEndAsync(cancellationToken);

        request.Body.Position = 0;

        return body;
    }
}
