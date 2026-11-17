
namespace ShopBridge.Application.Abstractions.Logging;

public interface ILogService
{
    Task LogApplicationAsync(
        object? request,
        object? response,
        TimeSpan executionTime,
        CancellationToken cancellationToken = default);

    Task LogBusinessErrorAsync(
        object? request,
        int errorCode,
        string errorMessage,
        CancellationToken cancellationToken = default);

    Task LogSystemErrorAsync(
        object? request,
        string errorMessage,
        string stackTrace,
        CancellationToken cancellationToken = default);
}

