using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.DTOs.Reminder;
using ProjectManagementService.Application.Features.Reminders.Commands;
using ProjectManagementService.Application.Features.Reminders.Queries;

namespace ProjectManagementService.API.Controllers;

/// <summary>
/// API quản lý nhắc hẹn cá nhân (Personal Calendar/Reminder)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ReminderController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ILogger<ReminderController> _logger;

    public ReminderController(IMediator mediator, ILogger<ReminderController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách nhắc hẹn của user hiện tại
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetMyReminders([FromQuery] bool includeExpired = false, [FromQuery] bool includeCompleted = false)
    {
        try
        {
            var query = new GetMyRemindersQuery
            {
                IncludeExpired = includeExpired,
                IncludeCompleted = includeCompleted
            };

            var result = await _mediator.Send(query);

            return Ok(new { success = true, data = result });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reminders");
            return BadRequest(new { success = false, message = "Có lỗi xảy ra" });
        }
    }

    /// <summary>
    /// Lấy chi tiết 1 nhắc hẹn
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetReminderById(Guid id)
    {
        try
        {
            var query = new GetReminderByIdQuery { Id = id };
            var result = await _mediator.Send(query);

            return Ok(new { success = true, data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting reminder");
            return BadRequest(new { success = false, message = "Có lỗi xảy ra" });
        }
    }

    /// <summary>
    /// Tạo nhắc hẹn mới
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateReminder([FromBody] CreateReminderDto dto)
    {
        try
        {
            var command = new CreateReminderCommand
            {
                Title = dto.Title,
                Note = dto.Note,
                ReminderTime = dto.ReminderTime
            };

            var result = await _mediator.Send(command);

            return Ok(new { success = true, message = "Tạo nhắc hẹn thành công", data = result });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating reminder");
            return BadRequest(new { success = false, message = "Có lỗi xảy ra" });
        }
    }

    /// <summary>
    /// Cập nhật nhắc hẹn
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateReminder(Guid id, [FromBody] UpdateReminderDto dto)
    {
        try
        {
            var command = new UpdateReminderCommand
            {
                Id = id,
                Title = dto.Title,
                Note = dto.Note,
                ReminderTime = dto.ReminderTime,
                IsCompleted = dto.IsCompleted
            };

            var result = await _mediator.Send(command);

            return Ok(new { success = true, message = "Cập nhật nhắc hẹn thành công", data = result });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating reminder");
            return BadRequest(new { success = false, message = "Có lỗi xảy ra" });
        }
    }

    /// <summary>
    /// Xóa nhắc hẹn
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteReminder(Guid id)
    {
        try
        {
            var command = new DeleteReminderCommand { Id = id };
            var result = await _mediator.Send(command);

            return Ok(new { success = true, message = "Xóa nhắc hẹn thành công" });
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { success = false, message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(new { success = false, message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting reminder");
            return BadRequest(new { success = false, message = "Có lỗi xảy ra" });
        }
    }
}
