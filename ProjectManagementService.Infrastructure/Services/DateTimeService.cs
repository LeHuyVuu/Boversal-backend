using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Infrastructure.Services;

/// <summary>
/// DateTime service implementation
/// Use this for better testability instead of DateTime.Now/UtcNow
/// </summary>
public class DateTimeService : IDateTimeService
{
    public DateTime Now => DateTime.Now;
    public DateTime UtcNow => DateTime.UtcNow;
    public DateOnly Today => DateOnly.FromDateTime(DateTime.Today);
}
