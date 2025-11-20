using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace SGBV.Infrastructure.API.Middlewares;

public class GlobalException(RequestDelegate next, ILogger<GlobalException> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var traceId = Guid.NewGuid().ToString();
        context.Response.Headers["trace-id"] = traceId;

        try
        {
            await next(context);
        }
        catch (ValidationException ex)
        {
            logger.LogError("Validation error captured: {Message} TraceId:{TraceId} Path:{Path}", ex.Message, traceId, context.Request.Path);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Errors",
                Detail = "One or more validation errors occurred.",
                Type = "ValidationFailure",
                Instance = context.Request.Path
            };

            problemDetails.Extensions["errors"] = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled general exception");

            var problemDetails = new ProblemDetails
            {
                Title = "Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
                Instance = context.Request.Path
            };

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
