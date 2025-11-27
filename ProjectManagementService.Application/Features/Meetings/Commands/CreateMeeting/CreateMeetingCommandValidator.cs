using FluentValidation;

namespace ProjectManagementService.Application.Features.Meetings.Commands.CreateMeeting;

/// <summary>
/// Validator cho CreateMeetingCommand
/// </summary>
public class CreateMeetingCommandValidator : AbstractValidator<CreateMeetingCommand>
{
    public CreateMeetingCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Tên meeting không được để trống")
            .MaximumLength(255).WithMessage("Tên meeting không được quá 255 ký tự");

        RuleFor(x => x.Description)
            .MaximumLength(2000).WithMessage("Mô tả không được quá 2000 ký tự");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Thời gian bắt đầu không được để trống")
            .GreaterThan(DateTime.UtcNow).WithMessage("Thời gian bắt đầu phải trong tương lai");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("Thời gian kết thúc không được để trống")
            .GreaterThan(x => x.StartTime).WithMessage("Thời gian kết thúc phải sau thời gian bắt đầu");

        RuleFor(x => x.MeetingLink)
            .MaximumLength(500).WithMessage("Link meeting không được quá 500 ký tự");

        RuleFor(x => x.Attendees)
            .NotEmpty().WithMessage("Phải có ít nhất 1 người tham gia");

        RuleForEach(x => x.Attendees)
            .EmailAddress().WithMessage("Email không hợp lệ");
    }
}
