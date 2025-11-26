namespace ProjectManagementService.Application.Interfaces;

/// <summary>
/// DateTime service for testable time operations
/// Use this instead of DateTime.Now/UtcNow for better testability
/// </summary>
public interface IDateTimeService
{
    DateTime Now { get; }
    DateTime UtcNow { get; }
    DateOnly Today { get; }
}
