namespace ShopBridge.Application.Abstractions.Logging;

public interface ILogMetadataResolver
{
    Task<int> GetActionIdAsync(
        string controllerName,
        string actionName,
        CancellationToken cancellationToken = default);
}
