using FluentValidation;
using ProjectManagementService.Application.Features.Auth.Commands;

namespace ProjectManagementService.Application.Features.Auth.Validators;

// Validator cho LoginCommand
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được trống")
            .EmailAddress().WithMessage("Email không đúng định dạng");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được trống")
            .MinimumLength(6).WithMessage("Password phải có ít nhất 6 ký tự");
    }
}
