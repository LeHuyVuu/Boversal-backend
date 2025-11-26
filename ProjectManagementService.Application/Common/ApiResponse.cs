namespace ProjectManagementService.Application.Common;

// Response có data
public class ApiResponse<T>
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public T? Data { get; set; }
    public List<string>? Errors { get; set; }

    // Tạo response thành công
    public static ApiResponse<T> Ok(T data, string message = "Thành công")
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    // Tạo response lỗi
    public static ApiResponse<T> Error(string message, List<string>? errors = null)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}

// Response không có data
public class ApiResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = "";
    public List<string>? Errors { get; set; }

    // Tạo response thành công
    public static ApiResponse Ok(string message = "Thành công")
    {
        return new ApiResponse
        {
            Success = true,
            Message = message
        };
    }

    // Tạo response lỗi
    public static ApiResponse Error(string message, List<string>? errors = null)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            Errors = errors
        };
    }
}
