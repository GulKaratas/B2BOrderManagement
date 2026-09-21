using System.Net;
using System.Text.Json;
using B2BOrderManagement.Api.Domain;

namespace B2BOrderManagement.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
        catch (BusinessException ex)
        {
            await WriteAsync(context, ex.StatusCode, ex.Code, ex.Message, ex.Details);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Beklenmeyen hata");
            await WriteAsync(context, (int)HttpStatusCode.InternalServerError, "INTERNAL_ERROR", "Beklenmeyen bir hata oluştu.", null);
        }
    }

    private static async Task WriteAsync(HttpContext context, int statusCode, string code, string message, object? details)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        var body = new { code, message, details };
        await context.Response.WriteAsync(JsonSerializer.Serialize(body, JsonOptions));
    }
}
