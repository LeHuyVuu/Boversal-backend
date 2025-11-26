using FluentValidation;
using ProjectManagementService.Application.Features.Tasks.Commands;

namespace ProjectManagementService.Application.Features.Tasks.Validators;

/// <summary>
/// Validator cho PatchTaskCommand
/// PATCH chỉ validate các fields được gửi lên (không null)
/// </summary>
public class PatchTaskCommandValidator : AbstractValidator<PatchTaskCommand>
{
    public PatchTaskCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Id phải lớn hơn 0");

        When(x => x.StatusId.HasValue, () =>
        {
            RuleFor(x => x.StatusId!.Value)
                .GreaterThan(0)
                .WithMessage("StatusId phải lớn hơn 0");
        });

        When(x => x.OrderIndex.HasValue, () =>
        {
            RuleFor(x => x.OrderIndex!.Value)
                .GreaterThanOrEqualTo(0)
                .WithMessage("OrderIndex phải lớn hơn hoặc bằng 0");
        });

        When(x => !string.IsNullOrEmpty(x.Priority), () =>
        {
            RuleFor(x => x.Priority)
                .Must(p => new[] { "low", "medium", "high", "urgent" }.Contains(p!.ToLower()))
                .WithMessage("Priority phải là: low, medium, high, hoặc urgent");
        });
    }
}
