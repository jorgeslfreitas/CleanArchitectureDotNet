using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Mostruario.Domain.Exceptions;

namespace Mostruario.Api.Middleware;

public class ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An unhandled exception occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        var (statusCode, message) = exception switch
        {
            NotFoundException => (404, exception.Message),
            BusinessRuleException => (400, exception.Message),
            _ => (500, "An internal error occurred. Please try again later.")
        };

        response.StatusCode = statusCode;

        var problem = new ProblemDetails
        {
            Status = statusCode,
            Title = message
        };

        var result = JsonSerializer.Serialize(problem);
        await response.WriteAsync(result);
    }
}