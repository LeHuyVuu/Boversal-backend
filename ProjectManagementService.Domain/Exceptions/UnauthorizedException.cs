namespace ProjectManagementService.Domain.Exceptions;

// Unauthorized Exception (HTTP 401)
public class UnauthorizedException : DomainException
{
    public UnauthorizedException(string message) : base(message)
    {
    }
}
