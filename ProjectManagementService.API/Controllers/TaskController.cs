using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Application.Features.Tasks.Commands;
using ProjectManagementService.Application.Features.Tasks.Queries;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.API.Controllers;

/// <summary>
/// Controller quản lý Tasks cho Kanban board
/// Tất cả APIs cần đăng nhập ([Authorize])
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TaskController : ControllerBase
{
    private readonly IMediator _mediator;

    public TaskController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy task theo ID
    /// </summary>
    /// <param name="id">ID của task</param>
    /// <returns>Thông tin chi tiết task</returns>
    /// <response code="200">Trả về thông tin task</response>
    /// <response code="404">Không tìm thấy task</response>
    [HttpGet("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse<TaskDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(long id)
    {
        var result = await _mediator.Send(new GetTaskByIdQuery(id));
        
        if (result == null)
            throw new NotFoundException($"Không tìm thấy task với id {id}");

        return Ok(ApiResponse<TaskDto>.Ok(result, "Lấy dữ liệu thành công"));
    }

    /// <summary>
    /// Lấy tất cả tasks trong 1 project (có phân trang)
    /// Dùng cho Kanban board hiển thị tất cả tasks
    /// </summary>
    /// <param name="projectId">ID của project</param>
    /// <param name="pageNumber">Số trang (mặc định: 1)</param>
    /// <param name="pageSize">Số items mỗi trang (mặc định: 50)</param>
    /// <returns>Danh sách tasks trong project</returns>
    /// <response code="200">Trả về danh sách tasks</response>
    [HttpGet("project/{projectId:long}")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetByProject(long projectId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetTasksByProjectQuery(projectId, pageNumber, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Lấy tasks của current user trong tất cả projects
    /// </summary>
    /// <param name="pageNumber">Số trang (mặc định: 1)</param>
    /// <param name="pageSize">Số items mỗi trang (mặc định: 50)</param>
    /// <returns>Danh sách tasks của user</returns>
    /// <response code="200">Trả về danh sách tasks</response>
    [HttpGet("my-tasks")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTasks([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetMyTasksQuery(pageNumber, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Lấy tasks của current user trong 1 project cụ thể
    /// Dùng cho Kanban board khi user filter "My Tasks"
    /// </summary>
    /// <param name="projectId">ID của project</param>
    /// <param name="pageNumber">Số trang (mặc định: 1)</param>
    /// <param name="pageSize">Số items mỗi trang (mặc định: 50)</param>
    /// <returns>Danh sách tasks của user trong project</returns>
    /// <response code="200">Trả về danh sách tasks</response>
    [HttpGet("project/{projectId:long}/my-tasks")]
    [ProducesResponseType(typeof(PaginatedResponse<TaskDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMyTasksByProject(long projectId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 50)
    {
        var result = await _mediator.Send(new GetMyTasksByProjectQuery(projectId, pageNumber, pageSize));
        return Ok(result);
    }

    /// <summary>
    /// Tạo task mới trong project
    /// </summary>
    /// <param name="command">Thông tin task cần tạo</param>
    /// <returns>ID của task vừa tạo</returns>
    /// <response code="201">Tạo task thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponse<long>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateTaskCommand command)
    {
        var createdId = await _mediator.Send(command);
        return CreatedAtAction(
            nameof(GetById),
            new { id = createdId },
            ApiResponse<long>.Ok(createdId, "Tạo task thành công"));
    }

    /// <summary>
    /// Cập nhật task (Full update)
    /// </summary>
    /// <param name="id">ID của task cần cập nhật</param>
    /// <param name="command">Thông tin mới</param>
    /// <returns>Kết quả cập nhật</returns>
    /// <response code="200">Cập nhật thành công</response>
    /// <response code="400">ID không khớp</response>
    /// <response code="404">Không tìm thấy task</response>
    [HttpPut("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(long id, [FromBody] UpdateTaskCommand command)
    {
        if (id != command.Id)
            throw new BusinessRuleException("Id không khớp");

        var result = await _mediator.Send(command);

        if (!result)
            throw new NotFoundException($"Không tìm thấy task với id {id}");

        return Ok(ApiResponse.Ok("Cập nhật thành công"));
    }

    /// <summary>
    /// PATCH task - chỉ cập nhật 1 vài fields
    /// Dùng cho Kanban khi kéo task:
    /// - Sang column khác: cập nhật StatusId
    /// - Thay đổi vị trí: cập nhật OrderIndex
    /// - Thay đổi priority: cập nhật Priority
    /// </summary>
    /// <param name="id">ID của task cần cập nhật</param>
    /// <param name="command">Các fields cần cập nhật (chỉ gửi fields thay đổi)</param>
    /// <returns>Kết quả cập nhật</returns>
    /// <response code="200">Cập nhật thành công</response>
    /// <response code="400">ID không khớp</response>
    /// <response code="404">Không tìm thấy task</response>
    [HttpPatch("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Patch(long id, [FromBody] PatchTaskCommand command)
    {
        if (id != command.Id)
            throw new BusinessRuleException("Id không khớp");

        var result = await _mediator.Send(command);

        if (!result)
            throw new NotFoundException($"Không tìm thấy task với id {id}");

        return Ok(ApiResponse.Ok("Cập nhật thành công"));
    }

    /// <summary>
    /// Xóa task (soft delete)
    /// </summary>
    /// <param name="id">ID của task cần xóa</param>
    /// <returns>Kết quả xóa</returns>
    /// <response code="200">Xóa thành công</response>
    /// <response code="404">Không tìm thấy task</response>
    [HttpDelete("{id:long}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(long id)
    {
        var result = await _mediator.Send(new DeleteTaskCommand(id));

        if (!result)
            throw new NotFoundException($"Không tìm thấy task với id {id}");

        return Ok(ApiResponse.Ok("Xóa thành công"));
    }
}
