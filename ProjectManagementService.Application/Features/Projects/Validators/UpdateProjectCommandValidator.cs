using FluentValidation;
using ProjectManagementService.Application.Features.Projects.Commands;

namespace ProjectManagementService.Application.Features.Projects.Validators;

public class UpdateProjectCommandValidator : AbstractValidator<UpdateProjectCommand>
{
    public UpdateProjectCommandValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Project ID must be greater than 0");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Project name is required")
            .MaximumLength(255)
            .WithMessage("Project name must not exceed 255 characters");

        RuleFor(x => x.DemoUrl)
            .MaximumLength(500)
            .WithMessage("Demo URL must not exceed 500 characters")
            .Must(BeValidUrl)
            .When(x => !string.IsNullOrEmpty(x.DemoUrl))
            .WithMessage("Demo URL must be a valid URL");

        RuleFor(x => x.ShortIntro)
            .MaximumLength(500)
            .WithMessage("Short intro must not exceed 500 characters");

        RuleFor(x => x.Highlight)
            .MaximumLength(500)
            .WithMessage("Highlight must not exceed 500 characters");

        RuleFor(x => x.Status)
            .MaximumLength(50)
            .WithMessage("Status must not exceed 50 characters");

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
