using MediatR;
using ProjectManagementService.Application.DTOs.Auth;

namespace ProjectManagementService.Application.Features.Auth.Commands;

// Command đăng nhập
public record LoginCommand(string Email, string Password) : IRequest<AuthResponseDto>;
