namespace ProjectManagementService.Domain.Exceptions;

// Lỗi khi không tìm thấy dữ liệu
public class NotFoundException : DomainException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
