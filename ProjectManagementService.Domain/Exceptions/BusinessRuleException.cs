namespace ProjectManagementService.Domain.Exceptions;

// Lỗi khi vi phạm quy tắc nghiệp vụ
// Ví dụ: Không thể xóa project đang có task
public class BusinessRuleException : DomainException
{
    public BusinessRuleException(string message) : base(message)
    {
    }
}
