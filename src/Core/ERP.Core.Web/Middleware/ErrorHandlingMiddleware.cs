using System.Net;
using System.Text.Json;
using ERP.Core.Web.Response;
using ERP.Core.Extensions;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace ERP.Core.Web.Middleware;

public sealed class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var response = context.Response;
        response.ContentType = "application/json";

        ApiResponse apiResponse;

        var errorResponse = exception switch
        {
            ValidationException validationException => new ApiResponse
            (
                "Validation failed",
                HttpStatusCode.BadRequest,
                validationException.Errors.GroupBy(p => p.PropertyName).ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                )
            ),
            ArgumentException argException => new ApiResponse
            (
                argException.Message,
                HttpStatusCode.BadRequest
            ),
            UnauthorizedAccessException unauthorizedAccessException => new ApiResponse
            (
                "Unauthorized access",
                HttpStatusCode.Unauthorized
            ),
            _ => new ApiResponse
            (
                "An internal server error occurred",
                HttpStatusCode.InternalServerError
            )
        };

        response.StatusCode = errorResponse?.Error?.Code?.ToConvertInt() ?? 500;
        await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
    }
}