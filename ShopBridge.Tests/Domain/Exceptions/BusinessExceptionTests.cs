
using FluentAssertions;
using ShopBridge.Domain.Exceptions;

namespace ShopBridge.Tests.Domain.Exceptions;

public class BusinessExceptionTests
{
    [Fact]
    public void Constructor_Should_Set_ErrorCode_And_ErrorType()
    {
        // Arrange
        const int errorCode = 1001;

        // Act
        var exception = new BusinessException(
            errorCode,
            "Product not found",
            BusinessErrorType.NotFound);

        // Assert
        exception.ErrorCode.Should().Be(errorCode);
        exception.ErrorType.Should().Be(BusinessErrorType.NotFound);
        exception.Message.Should().Be("Product not found");
    }

    [Fact]
    public void Constructor_Should_Use_General_ErrorType_By_Default()
    {
        // Act
        var exception = new BusinessException(
            1001,
            "Something went wrong");

        // Assert 
        exception.ErrorType.Should().Be(BusinessErrorType.General);
    }

    [Fact]
    public void Constructor_Should_Throw_When_ErrorCode_Is_Not_Positive()
    {
        // Act
        Action act = () => new BusinessException(
            0,
            "Invalid error code");

        // Assert
        act.Should()
            .Throw<ArgumentOutOfRangeException>()
            .WithParameterName("errorCode");
    }

    [Fact]
    public void Throw_Should_Throw_BusinessException()
    {
        // Act
        Action act = () => BusinessException.Throw(
            1001,
            "Product not found",
            BusinessErrorType.NotFound);

        // Assert
        var exception = act.Should()
            .Throw<BusinessException>();

        exception.Which.ErrorCode.Should().Be(1001);
        exception.Which.ErrorType.Should().Be(BusinessErrorType.NotFound);
    }
}
