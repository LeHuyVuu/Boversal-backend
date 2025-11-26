using FluentValidation;
using ProjectManagementService.Application.Features.Auth.Commands;

namespace ProjectManagementService.Application.Features.Auth.Validators;

// Validator cho RegisterCommand
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email không được trống")
            .EmailAddress().WithMessage("Email không đúng định dạng")
            .MaximumLength(100).WithMessage("Email không quá 100 ký tự");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password không được trống")
            .MinimumLength(6).WithMessage("Password phải có ít nhất 6 ký tự")
            .MaximumLength(100).WithMessage("Password không quá 100 ký tự");

        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Họ tên không được trống")
            .MaximumLength(200).WithMessage("Họ tên không quá 200 ký tự");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Số điện thoại không quá 20 ký tự")
            .When(x => !string.IsNullOrEmpty(x.PhoneNumber));
    }
}
