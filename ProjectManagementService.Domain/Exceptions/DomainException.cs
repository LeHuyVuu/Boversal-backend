namespace ProjectManagementService.Domain.Exceptions;

// Exception cơ bản cho Domain
public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }
}
