using System.Diagnostics;

namespace ProjectManagementService.API.Middleware;

// Thêm Request ID và đo thời gian xử lý
public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Tạo Request ID
        var requestId = Guid.NewGuid().ToString();
        context.Response.Headers["X-Request-ID"] = requestId;

        var stopwatch = Stopwatch.StartNew();

        _logger.LogInformation(
            "Bắt đầu: {Method} {Path} (ID: {RequestId})",
            context.Request.Method,
            context.Request.Path,
            requestId);

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            _logger.LogInformation(
                "Hoàn thành: {Method} {Path} - Status: {StatusCode} - Thời gian: {Duration}ms (ID: {RequestId})",
                context.Request.Method,
                context.Request.Path,
                context.Response.StatusCode,
                stopwatch.ElapsedMilliseconds,
                requestId);
        }
    }
}