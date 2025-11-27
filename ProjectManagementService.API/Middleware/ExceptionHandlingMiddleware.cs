using ProjectManagementService.Application.Common;
using ProjectManagementService.Domain.Exceptions;
using System.Net;
using System.Text.Json;
using ValidationException = ProjectManagementService.Domain.Exceptions.ValidationException;

namespace ProjectManagementService.API.Middleware;

// Bắt tất cả lỗi và trả về response chuẩn
public class ExceptionHandlingMiddleware
{
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";

        // Xác định loại lỗi và status code
        var (statusCode, message, errors) = exception switch
        {
            ValidationException validationEx => (
                HttpStatusCode.BadRequest,
                "Dữ liệu không hợp lệ",
                validationEx.Errors.SelectMany(e => e.Value.Select(v => $"{e.Key}: {v}")).ToList()
            ),
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                notFoundEx.Message,
                null
            ),
            BusinessRuleException businessEx => (
                HttpStatusCode.BadRequest,
                businessEx.Message,
                null
            ),
            ConflictException conflictEx => (
                HttpStatusCode.Conflict,
                conflictEx.Message,
                null
            ),
            ForbiddenException forbiddenEx => (
                HttpStatusCode.Forbidden,
                forbiddenEx.Message,
                null
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "Có lỗi xảy ra",
                new List<string> { exception.Message }
            )
        };

        context.Response.StatusCode = (int)statusCode;

        // Log lỗi
        if ((int)statusCode >= 500)
            _logger.LogError(exception, "Lỗi server: {Message}", exception.Message);
        else
            _logger.LogWarning("Lỗi client: {Message}", exception.Message);

        // Trả về response
        var response = ApiResponse.Error(message, errors);
        var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
            WriteIndented = true
        });

        await context.Response.WriteAsync(json);
    }
}