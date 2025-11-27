using FluentValidation;
using ProjectManagementService.Application.Features.Projects.Commands;

namespace ProjectManagementService.Application.Features.Projects.Validators;

public class CreateProjectCommandValidator : AbstractValidator<CreateProjectCommand>
{
    public CreateProjectCommandValidator()
    {
        RuleFor(x => x.OwnerId)
            .GreaterThan(0)
            .WithMessage("OwnerId must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .MaximumLength(255)
            .WithMessage("Project name must not exceed 255 characters");

        RuleFor(x => x.DemoUrl)
            .MaximumLength(500)
            .WithMessage("Demo URL must not exceed 500 characters")
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrWhiteSpace(x.DemoUrl))
            .WithMessage("Demo URL must be a valid URL");

        RuleFor(x => x.ShortIntro)
            .MaximumLength(500)
            .WithMessage("Short intro must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.ShortIntro));

        RuleFor(x => x.Highlight)
            .MaximumLength(500)
            .WithMessage("Highlight must not exceed 500 characters")
            .When(x => !string.IsNullOrWhiteSpace(x.Highlight));

        RuleFor(x => x.EndDate)
            .GreaterThanOrEqualTo(x => x.StartDate)
            .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
            .WithMessage("End date must be greater than or equal to start date");
    }

    private bool BeValidUrl(string? url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out var uriResult)
            && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
    }
}
