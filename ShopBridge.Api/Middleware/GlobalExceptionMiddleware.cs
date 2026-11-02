using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ShopBridge.Api.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred.");

            await HandleExceptionAsync(context, ex);
        }

    }

    private static async Task HandleExceptionAsync(
        HttpContext httpContext,
        Exception ex)
    {
        var (statusCode, title) = ex switch
        {
            ValidationException =>
                (StatusCodes.Status400BadRequest, "Validation Error"),

            KeyNotFoundException =>
                (StatusCodes.Status404NotFound, "Resource Not Found"),

            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error")
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/problem+json";

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = statusCode == 500
                ? "An unexpected error occurred."
                : ex.Message,
            Instance = httpContext.Request.Path
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails);
    }
}
