using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Infrastructure.Services;

/// <summary>
/// Background Service để:
/// 1. Gửi email nhắc trước 15 phút
/// 2. Đánh dấu reminder đã hết hạn (sau thời gian hẹn)
/// </summary>
public class ReminderBackgroundService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderBackgroundService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(5); // Check mỗi 5 phút

    public ReminderBackgroundService(
        IServiceProvider serviceProvider,
        ILogger<ReminderBackgroundService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("ReminderBackgroundService is starting.");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessRemindersAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred in ReminderBackgroundService");
            }

            // Đợi trước khi check lần tiếp theo
            await Task.Delay(_checkInterval, stoppingToken);
        }

        _logger.LogInformation("ReminderBackgroundService is stopping.");
    }

    private async Task ProcessRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var reminderRepository = scope.ServiceProvider.GetRequiredService<IReminderRepository>();
        var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

        // 1. Gửi email cho các reminder cần nhắc (trước 15 phút)
        var pendingReminders = await reminderRepository.GetPendingEmailRemindersAsync(cancellationToken);
        
        _logger.LogInformation($"Found {pendingReminders.Count} reminders pending email notification");

        foreach (var reminder in pendingReminders)
        {
            try
            {
                var timeUntilReminder = reminder.ReminderTime - DateTime.UtcNow;
                var minutesUntil = (int)timeUntilReminder.TotalMinutes;

                var emailBody = $@"
                    <h2>🔔 Nhắc nhở: {reminder.Title}</h2>
                    <p><strong>Thời gian:</strong> {reminder.ReminderTime:dd/MM/yyyy HH:mm}</p>
                    <p><strong>Còn {minutesUntil} phút nữa!</strong></p>
                    {(string.IsNullOrEmpty(reminder.Note) ? "" : $"<p><strong>Ghi chú:</strong> {reminder.Note}</p>")}
                    <hr/>
                    <p>Đây là email nhắc nhở tự động từ hệ thống Boversal.</p>
                ";

                await emailService.SendEmailAsync(
                    to: reminder.User.Email,
                    subject: $"🔔 Nhắc nhở: {reminder.Title}",
                    body: emailBody
                );

                // Đánh dấu đã gửi email
                await reminderRepository.MarkAsEmailSentAsync(reminder.Id, cancellationToken);

                _logger.LogInformation($"Sent reminder email to {reminder.User.Email} for reminder: {reminder.Title}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send reminder email for ID: {reminder.Id}");
            }
        }

        // 2. Đánh dấu các reminder đã hết hạn (sau thời gian hẹn)
        var now = DateTime.UtcNow;
        var upcomingReminders = await reminderRepository.GetUpcomingRemindersAsync(DateTime.MinValue, cancellationToken);
        
        var expiredIds = upcomingReminders
            .Where(r => r.ReminderTime < now && !r.IsExpired)
            .Select(r => r.Id)
            .ToList();

        if (expiredIds.Any())
        {
            await reminderRepository.MarkAsExpiredAsync(expiredIds, cancellationToken);
            _logger.LogInformation($"Marked {expiredIds.Count} reminders as expired");
        }
    }
}
