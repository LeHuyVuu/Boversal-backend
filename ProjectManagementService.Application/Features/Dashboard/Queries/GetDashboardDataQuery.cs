using MediatR;
using ProjectManagementService.Application.DTOs.Dashboard;

namespace ProjectManagementService.Application.Features.Dashboard.Queries;

public record GetDashboardDataQuery : IRequest<DashboardDataDto>;
