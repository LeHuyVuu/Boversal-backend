namespace ProjectManagementService.Domain.Exceptions;

// Lỗi khi validate dữ liệu
public class ValidationException : DomainException
{
    public Dictionary<string, string[]> Errors { get; }

    public ValidationException(string message) : base(message)
    {
        Errors = new Dictionary<string, string[]>();
    }
    
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("Dữ liệu không hợp lệ")
    {
        Errors = errors;
    }
}
