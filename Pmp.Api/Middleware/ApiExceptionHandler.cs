using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Pmp.Api.Middleware;

internal sealed class ApiExceptionHandler(
    ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            ArgumentException => (
                StatusCodes.Status400BadRequest,
                "Invalid request",
                "The request contains invalid data."),
            InvalidOperationException => (
                StatusCodes.Status409Conflict,
                "Operation could not be completed",
                "The requested operation conflicts with the current project state."),
            _ => (
                StatusCodes.Status500InternalServerError,
                "Unexpected error",
                "The server could not complete the request.")
        };

        logger.LogError(exception, "API request failed with status code {StatusCode}.", statusCode);

        httpContext.Response.StatusCode = statusCode;
        await httpContext.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        }, cancellationToken);

        return true;
    }
}
