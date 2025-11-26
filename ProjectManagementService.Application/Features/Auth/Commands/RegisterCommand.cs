using MediatR;
using ProjectManagementService.Application.DTOs.Auth;

namespace ProjectManagementService.Application.Features.Auth.Commands;

// Command đăng ký tài khoản mới
public record RegisterCommand(
    string Email, 
    string Password, 
    string FullName, 
    string? PhoneNumber
) : IRequest<AuthResponseDto>;
