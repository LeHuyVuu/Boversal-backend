using FluentValidation;
using ProjectManagementService.Application.Features.Users.Commands;

namespace ProjectManagementService.Application.Features.Users.Validators;

/// <summary>
/// Validator cho UpdateUserProfileCommand
/// </summary>
public class UpdateUserProfileCommandValidator : AbstractValidator<UpdateUserProfileCommand>
{
    public UpdateUserProfileCommandValidator()
    {
        RuleFor(x => x.FullName)
            .MaximumLength(200)
            .WithMessage("Họ tên không được vượt quá 200 ký tự")
            .When(x => !string.IsNullOrEmpty(x.FullName));

        RuleFor(x => x.Gender)
            .Must(g => string.IsNullOrEmpty(g) || new[] { "male", "female", "other" }.Contains(g.ToLower()))
            .WithMessage("Gender phải là: male, female, hoặc other")
            .When(x => !string.IsNullOrEmpty(x.Gender));

        RuleFor(x => x.Phone)
            .MaximumLength(20)
            .WithMessage("Số điện thoại không được vượt quá 20 ký tự")
            .Matches(@"^[\d\s\+\-\(\)]+$")
            .WithMessage("Số điện thoại chỉ được chứa số và ký tự đặc biệt: + - ( ) space")
            .When(x => !string.IsNullOrEmpty(x.Phone));

        RuleFor(x => x.Address)
            .MaximumLength(500)
            .WithMessage("Địa chỉ không được vượt quá 500 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Address));

        RuleFor(x => x.Bio)
            .MaximumLength(1000)
            .WithMessage("Bio không được vượt quá 1000 ký tự")
            .When(x => !string.IsNullOrEmpty(x.Bio));

        RuleFor(x => x.AvatarUrl)
            .MaximumLength(1000)
            .WithMessage("Avatar URL không được vượt quá 1000 ký tự")
            .Must(url => string.IsNullOrEmpty(url) || Uri.TryCreate(url, UriKind.Absolute, out _))
            .WithMessage("Avatar URL phải là URL hợp lệ")
            .When(x => !string.IsNullOrEmpty(x.AvatarUrl));
    }
}
