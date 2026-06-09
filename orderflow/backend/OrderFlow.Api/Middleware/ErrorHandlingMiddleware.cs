using System.Net;
using System.Text.Json;
using OrderFlow.Api.DTOs;
using OrderFlow.Api.Exceptions;

namespace OrderFlow.Api.Middleware;

public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
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
        catch (NotFoundException ex)
        {
            await WriteErrorAsync(context, ex.Message, HttpStatusCode.NotFound);
        }
        catch (BusinessRuleException ex)
        {
            await WriteErrorAsync(context, ex.Message, HttpStatusCode.BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled API error");
            await WriteErrorAsync(context, "An unexpected error occurred.", HttpStatusCode.InternalServerError);
        }
    }

    private static async Task WriteErrorAsync(HttpContext context, string message, HttpStatusCode statusCode)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ApiErrorResponse(
            message,
            (int)statusCode,
            context.TraceIdentifier);

        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(json);
    }
}
