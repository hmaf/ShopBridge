using System.Diagnostics.CodeAnalysis;

namespace ShopBridge.Domain.Exceptions;

public class BusinessException : Exception
{
    public int ErrorCode { get; }

    public BusinessException(
        int errorCode,
        string message) : base(message)
    {
        if (errorCode <= 0)
            throw new ArgumentOutOfRangeException(nameof(errorCode));
        
        ErrorCode = errorCode;
    }

    [DoesNotReturn]
    public static void Throw(int errorCode, string message)
        => throw new BusinessException(errorCode, message);

}
