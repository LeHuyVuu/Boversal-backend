using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;

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
            // Log presence of Authorization header and jwt cookie (without printing secret)
            var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
            if (!string.IsNullOrEmpty(authHeader) && authHeader.StartsWith("Bearer "))
            {
                var token = authHeader.Substring("Bearer ".Length).Trim();
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(token);
                    var alg = jwt.Header.Alg ?? "(unknown)";
                    var iss = jwt.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
                    var aud = jwt.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
                    _logger.LogInformation("Auth header present: token-length={Length} alg={Alg} iss={Iss} aud={Aud}", token.Length, alg, iss ?? "(none)", aud ?? "(none)");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Auth header present but token could not be read: {Message}", ex.Message);
                }
            }
            else
            {
                _logger.LogDebug("No Authorization header present");
            }

            var jwtCookie = context.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(jwtCookie))
            {
                try
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwt = handler.ReadJwtToken(jwtCookie);
                    var alg = jwt.Header.Alg ?? "(unknown)";
                    var iss = jwt.Claims.FirstOrDefault(c => c.Type == "iss")?.Value;
                    var aud = jwt.Claims.FirstOrDefault(c => c.Type == "aud")?.Value;
                    _logger.LogInformation("Cookie jwt present: token-length={Length} alg={Alg} iss={Iss} aud={Aud}", jwtCookie.Length, alg, iss ?? "(none)", aud ?? "(none)");
                }
                catch (Exception ex)
                {
                    _logger.LogInformation("Cookie jwt present but token could not be read: {Message}", ex.Message);
                }
            }
            else
            {
                // Also log raw Cookie header for debugging (some proxies may prevent cookie parsing)
                var rawCookieHeader = context.Request.Headers["Cookie"].FirstOrDefault();
                _logger.LogDebug("No jwt cookie present. Raw Cookie header='{CookieHeader}'", rawCookieHeader ?? "(none)");
            }

            // Additional headers useful for debugging cross-site cookie issues
            var host = context.Request.Headers["Host"].FirstOrDefault();
            var origin = context.Request.Headers["Origin"].FirstOrDefault();
            var referer = context.Request.Headers["Referer"].FirstOrDefault();
            var forwardedProto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
            _logger.LogDebug("Request routing info: Host={Host} Origin={Origin} Referer={Referer} X-Forwarded-Proto={FwdProto}", host ?? "(none)", origin ?? "(none)", referer ?? "(none)", forwardedProto ?? "(none)");

            // Log ALL headers and cookies for full debug
            _logger.LogInformation("ALL HEADERS: {Headers}", string.Join("; ", context.Request.Headers.Select(h => h.Key + "=" + h.Value)));
            _logger.LogInformation("ALL COOKIES: {Cookies}", string.Join("; ", context.Request.Cookies.Select(c => c.Key + "=" + c.Value)));

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