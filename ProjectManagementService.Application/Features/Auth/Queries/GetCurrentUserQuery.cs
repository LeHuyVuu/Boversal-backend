using MediatR;
using ProjectManagementService.Application.DTOs.Auth;

namespace ProjectManagementService.Application.Features.Auth.Queries;

// Query lấy thông tin user đang đăng nhập
public record GetCurrentUserQuery : IRequest<UserDto>;
