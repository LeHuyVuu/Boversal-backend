using UtilityService.Messaging;

namespace UtilityService.Infrastructure;

public interface IEmailService
{
    Task SendMeetingInvitationAsync(MeetingCreatedEvent meetingEvent, string recipientEmail);
}
