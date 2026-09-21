namespace B2BOrderManagement.Api.Domain;

public class BusinessException : Exception
{
    public string Code { get; }
    public int StatusCode { get; }
    public object? Details { get; }

    public BusinessException(string code, string message, int statusCode = 422, object? details = null)
        : base(message)
    {
        Code = code;
        StatusCode = statusCode;
        Details = details;
    }

    public static BusinessException NotFound(string entity, object id) =>
        new("NOT_FOUND", $"{entity} bulunamadı.", 404, new { id });

    public static BusinessException Validation(string message, object? details = null) =>
        new("VALIDATION_ERROR", message, 400, details);

    public static BusinessException Conflict(string code, string message, object? details = null) =>
        new(code, message, 409, details);

    public static BusinessException Unauthorized(string message) =>
        new("UNAUTHORIZED", message, 401);
}
