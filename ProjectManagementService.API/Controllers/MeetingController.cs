using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.DTOs.Meeting;
using ProjectManagementService.Application.Features.Meetings.Commands.CreateMeeting;
using ProjectManagementService.Application.Features.Meetings.Commands.DeleteMeeting;
using ProjectManagementService.Application.Features.Meetings.Commands.UpdateMeeting;
using ProjectManagementService.Application.Features.Meetings.Queries.GetMeetingById;
using ProjectManagementService.Application.Features.Meetings.Queries.GetMeetings;

namespace ProjectManagementService.API.Controllers;

/// <summary>
/// Controller quản lý Meeting
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class MeetingController : ControllerBase
{
    private readonly IMediator _mediator;

    public MeetingController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy danh sách Meeting của user hiện tại
    /// </summary>
    [HttpGet]
    public async Task<ActionResult<List<MeetingDto>>> GetMyMeetings()
    {
        var query = new GetMeetingsQuery();
        var result = await _mediator.Send(query);
        return Ok(result);
    }

    /// <summary>
    /// Lấy Meeting theo ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<MeetingDto>> GetMeetingById(long id)
    {
        var query = new GetMeetingByIdQuery { MeetingId = id };
        var result = await _mediator.Send(query);

        if (result == null)
        {
            return NotFound(new { message = "Meeting không tồn tại hoặc bạn không có quyền truy cập" });
        }

        return Ok(result);
    }

    /// <summary>
    /// Tạo Meeting mới
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<MeetingDto>> CreateMeeting([FromBody] CreateMeetingDto dto)
    {
        var command = new CreateMeetingCommand
        {
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            MeetingLink = dto.MeetingLink,
            Attendees = dto.Attendees
        };

        var result = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetMeetingById), new { id = result.Id }, result);
    }

    /// <summary>
    /// Update Meeting
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult> UpdateMeeting(long id, [FromBody] UpdateMeetingDto dto)
    {
        var command = new UpdateMeetingCommand
        {
            MeetingId = id,
            Title = dto.Title,
            Description = dto.Description,
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            MeetingLink = dto.MeetingLink,
            Attendees = dto.Attendees
        };

        try
        {
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { message = "Cập nhật thất bại" });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }

    /// <summary>
    /// Xóa Meeting (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<ActionResult> DeleteMeeting(long id)
    {
        var command = new DeleteMeetingCommand { MeetingId = id };

        try
        {
            var result = await _mediator.Send(command);
            
            if (!result)
            {
                return NotFound(new { message = "Xóa thất bại" });
            }

            return NoContent();
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (UnauthorizedAccessException ex)
        {
            return Forbid();
        }
    }
}
