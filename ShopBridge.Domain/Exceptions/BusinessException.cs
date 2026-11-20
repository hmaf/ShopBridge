using System.Diagnostics.CodeAnalysis;

namespace ShopBridge.Domain.Exceptions;

public class BusinessException : Exception
{
    public int ErrorCode { get; }
    public BusinessErrorType ErrorType { get; }

    public BusinessException(
        int errorCode,
        string message,
        BusinessErrorType errorType = BusinessErrorType.General)
        : base(message)
    {
        if (errorCode <= 0)
            throw new ArgumentOutOfRangeException(nameof(errorCode));
        
        ErrorCode = errorCode;
        ErrorType = errorType;
    }

    [DoesNotReturn]
    public static void Throw(
        int errorCode, 
        string message,
        BusinessErrorType errorType = BusinessErrorType.General)
        => throw new BusinessException(errorCode, message, errorType);

}
