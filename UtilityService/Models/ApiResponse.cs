namespace UtilityService.Models;

public record ApiResponse<T>(
    int Status,
    string Message,
    T? Data,
    string? Errors,
    DateTimeOffset Timestamp
)
{
    public static ApiResponse<T> Success(T data, string message = "Success") =>
        new(200, message, data, null, DateTimeOffset.UtcNow);

    public static ApiResponse<T> Fail(int status, string message, string? errors = null) =>
        new(status, message, default, errors, DateTimeOffset.UtcNow);
}