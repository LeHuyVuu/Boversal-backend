namespace ProjectManagementService.Domain.Exceptions;

// Lỗi khi không có quyền truy cập
public class ForbiddenException : DomainException
{
    public ForbiddenException(string message = "Không có quyền truy cập") : base(message)
    {
    }
}
