using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Serilog.Context;
using ShopBridge.Application.Abstractions.Logging;
using System.Security.Claims;
using System.Text.Json;

namespace ShopBridge.Infra.Logging.Services;

public class LogService : ILogService
{
    private readonly ILogger<LogService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public LogService(
        ILogger<LogService> logger,
        IHttpContextAccessor httpContextAccessor)
    {
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
    }

    public Task LogApplicationAsync(
        object? request,
        object? response,
        TimeSpan executionTime,
        CancellationToken cancellationToken = default)
    {
        WriteLog(
            LogType.Application,
            LogLevel.Information,
            request,
            response,
            executionTime: executionTime);

        return Task.CompletedTask;
    }

    public Task LogBusinessErrorAsync(
        object? request,
        int errorCode,
        string errorMessage,
        CancellationToken cancellationToken = default)
    {
        using (LogContext.PushProperty("ErrorCode", errorCode))
        using (LogContext.PushProperty("ErrorMessage", errorMessage))
        {
            WriteLog(
                LogType.BusinessError,
                LogLevel.Warning,
                request);
        }

        return Task.CompletedTask;
    }

    public Task LogSystemErrorAsync(
        object? request,
        string errorMessage,
        string stackTrace,
        CancellationToken cancellationToken = default)
    {
        using (LogContext.PushProperty("ErrorMessage", errorMessage))
        using (LogContext.PushProperty("StackTrace", stackTrace))
        {
            WriteLog(
                LogType.SystemError,
                LogLevel.Error,
                request);
        }

        return Task.CompletedTask;
    }

    private void WriteLog(
        string logType,
        LogLevel level,
        object? request,
        object? response = null,
        TimeSpan? executionTime = null)
    {
        var context = _httpContextAccessor.HttpContext;

        var actionId = context?.Items["ActionId"] is int id
            ? id
            : (int?)null;

        if (actionId is null || actionId <= 0)
        {
            throw new InvalidOperationException(
                "A valid ActionId is required to persist the log.");
        }

        var userId = context?.User?
            .FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var userIdValue = Guid.TryParse(userId, out var parsedUserId)
            ? parsedUserId
            : (Guid?)null;

        using (LogContext.PushProperty("ActionId", actionId.Value))
        using (LogContext.PushProperty("LogType", logType))
        using (LogContext.PushProperty("UserId", userIdValue))
        using (LogContext.PushProperty(
            "Request",
            JsonSerializer.Serialize(request)))
        {
            if (logType == LogType.Application)
            {
                using (LogContext.PushProperty(
                    "Response",
                    JsonSerializer.Serialize(response)))
                using (LogContext.PushProperty(
                    "ExecutionTime",
                    executionTime))
                {
                    _logger.Log(
                        level,
                        "API log: {LogType}, ActionId: {ActionId}",
                        logType,
                        actionId.Value);
                }
            }
            else
            {
                _logger.Log(
                    level,
                    "API log: {LogType}, ActionId: {ActionId}",
                    logType,
                    actionId.Value);
            }
        }
    }
}