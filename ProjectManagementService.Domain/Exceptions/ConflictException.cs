namespace ProjectManagementService.Domain.Exceptions;

// Lỗi khi dữ liệu bị trùng lặp
// Ví dụ: Username đã tồn tại, Email đã đăng ký
public class ConflictException : DomainException
{
    public ConflictException(string message) : base(message)
    {
    }
}
