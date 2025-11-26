using FluentValidation;
using ProjectManagementService.Application.Features.Users.Commands;

namespace ProjectManagementService.Application.Features.Users.Validators;

/// <summary>
/// Validator cho ChangePasswordCommand
/// </summary>
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Mật khẩu hiện tại không được để trống");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Mật khẩu mới không được để trống")
            .MinimumLength(6)
            .WithMessage("Mật khẩu mới phải có ít nhất 6 ký tự")
            .MaximumLength(100)
            .WithMessage("Mật khẩu mới không được vượt quá 100 ký tự")
            .Matches(@"[A-Z]")
            .WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ hoa")
            .Matches(@"[a-z]")
            .WithMessage("Mật khẩu mới phải chứa ít nhất 1 chữ thường")
            .Matches(@"[0-9]")
            .WithMessage("Mật khẩu mới phải chứa ít nhất 1 số")
            .Matches(@"[\W_]")
            .WithMessage("Mật khẩu mới phải chứa ít nhất 1 ký tự đặc biệt");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Xác nhận mật khẩu không được để trống")
            .Equal(x => x.NewPassword)
            .WithMessage("Xác nhận mật khẩu không khớp với mật khẩu mới");

        RuleFor(x => x.NewPassword)
            .NotEqual(x => x.CurrentPassword)
            .WithMessage("Mật khẩu mới phải khác mật khẩu hiện tại")
            .When(x => !string.IsNullOrEmpty(x.CurrentPassword) && !string.IsNullOrEmpty(x.NewPassword));
    }
}
