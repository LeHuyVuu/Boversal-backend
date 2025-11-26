using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.DTOs.Dashboard;
using ProjectManagementService.Application.Features.Dashboard.Queries;

namespace ProjectManagementService.API.Controllers;

/// <summary>
/// Controller để quản lý Dashboard data
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IMediator _mediator;

    public DashboardController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy tất cả dữ liệu cho Dashboard
    /// </summary>
    /// <returns>Dashboard data bao gồm stats, recent tasks, và active projects</returns>
    /// <response code="200">Trả về dashboard data</response>
    /// <response code="401">Unauthorized - Cần đăng nhập</response>
    [HttpGet]
    [ProducesResponseType(typeof(Application.Common.ApiResponse<DashboardDataDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<ActionResult<Application.Common.ApiResponse<DashboardDataDto>>> GetDashboardData()
    {
        var data = await _mediator.Send(new GetDashboardDataQuery());
        return Ok(Application.Common.ApiResponse<DashboardDataDto>.Ok(data, "Lấy dữ liệu dashboard thành công"));
    }
}
