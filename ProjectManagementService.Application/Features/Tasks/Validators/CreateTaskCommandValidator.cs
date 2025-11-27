using FluentValidation;
using ProjectManagementService.Application.Features.Tasks.Commands;

namespace ProjectManagementService.Application.Features.Tasks.Validators;

/// <summary>
/// Validator cho CreateTaskCommand
/// </summary>
public class CreateTaskCommandValidator : AbstractValidator<CreateTaskCommand>
{
    public CreateTaskCommandValidator()
    {
        RuleFor(x => x.ProjectId)
            .GreaterThan(0)
            .WithMessage("ProjectId phải lớn hơn 0");

        // Title is REQUIRED in database
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title không được để trống")
            .MaximumLength(500)
            .WithMessage("Title không được vượt quá 500 ký tự");

        // Description is optional (mediumtext in database)
        RuleFor(x => x.Description)
            .MaximumLength(5000)
            .WithMessage("Description không được vượt quá 5000 ký tự")
            .When(x => !string.IsNullOrWhiteSpace(x.Description));

        RuleFor(x => x.Priority)
            .Must(p => string.IsNullOrEmpty(p) || new[] { "low", "medium", "high", "emergency" }.Contains(p.ToLower()))
            .WithMessage("Priority phải là: low, medium, high, hoặc emergency");

        RuleFor(x => x.OrderIndex)
            .GreaterThanOrEqualTo(0)
            .WithMessage("OrderIndex phải lớn hơn hoặc bằng 0");
    }
}
