
namespace ShopBridge.Application.Common.Mapping;

public class ApiResponse<T>
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public T? Data { get; init; }

    public static ApiResponse<T> SuccessResponse(
        T? data,
        string message = "Operation completed successfully.")
        => new()
        {
            Success = true,
            Message = message,
            Data = data
        };

    public static ApiResponse<T> FailureResponse(string message)
        => new()
        {
            Success = false,
            Message = message
        };
}
