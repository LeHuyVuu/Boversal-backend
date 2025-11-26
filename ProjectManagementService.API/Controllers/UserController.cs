using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.User;
using ProjectManagementService.Application.Features.Users.Commands;
using ProjectManagementService.Application.Features.Users.Queries;

namespace ProjectManagementService.API.Controllers;

/// <summary>
/// Controller quản lý User Profile (GetById, UpdateProfile, ChangePassword, Search)
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UserController : ControllerBase
{
    private readonly IMediator _mediator;

    public UserController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Lấy thông tin user theo ID
    /// Dùng để xem profile của user khác trong team
    /// </summary>
    /// <param name="id">User ID</param>
    /// <returns>Thông tin chi tiết user</returns>
    /// <response code="200">Trả về thông tin user</response>
    /// <response code="404">Không tìm thấy user</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserById(long id)
    {
        var result = await _mediator.Send(new GetUserByIdQuery { UserId = id });
        return Ok(ApiResponse<UserProfileDto>.Ok(result, "Lấy thông tin user thành công"));
    }

    /// <summary>
    /// Cập nhật thông tin profile của user hiện tại
    /// Chỉ cập nhật các field được gửi lên (partial update)
    /// </summary>
    /// <param name="dto">Thông tin profile cần cập nhật</param>
    /// <returns>Thông tin profile đã cập nhật</returns>
    /// <response code="200">Cập nhật profile thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpPut("profile")]
    [ProducesResponseType(typeof(ApiResponse<UserProfileDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateUserProfileDto dto)
    {
        var command = new UpdateUserProfileCommand
        {
            FullName = dto.FullName,
            Gender = dto.Gender,
            Phone = dto.Phone,
            Address = dto.Address,
            Bio = dto.Bio,
            AvatarUrl = dto.AvatarUrl
        };

        var result = await _mediator.Send(command);
        return Ok(ApiResponse<UserProfileDto>.Ok(result, "Cập nhật profile thành công"));
    }

    /// <summary>
    /// Đổi password cho user hiện tại
    /// Yêu cầu nhập password hiện tại, password mới và confirm password
    /// </summary>
    /// <param name="dto">Thông tin đổi password</param>
    /// <returns>Kết quả đổi password</returns>
    /// <response code="200">Đổi password thành công</response>
    /// <response code="400">Dữ liệu không hợp lệ hoặc password hiện tại sai</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpPut("password")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        var command = new ChangePasswordCommand
        {
            CurrentPassword = dto.CurrentPassword,
            NewPassword = dto.NewPassword,
            ConfirmPassword = dto.ConfirmPassword
        };

        await _mediator.Send(command);
        return Ok(ApiResponse.Ok("Đổi mật khẩu thành công"));
    }

    /// <summary>
    /// Tìm kiếm user theo tên hoặc email
    /// Dùng cho dropdown chọn assignee khi tạo task
    /// </summary>
    /// <param name="q">Từ khóa tìm kiếm (tên hoặc email)</param>
    /// <param name="limit">Số lượng kết quả tối đa (mặc định 10)</param>
    /// <returns>Danh sách user tìm được</returns>
    /// <response code="200">Trả về danh sách user</response>
    /// <response code="401">Chưa đăng nhập</response>
    [HttpGet("search")]
    [ProducesResponseType(typeof(ApiResponse<List<SearchUserDto>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> SearchUsers([FromQuery] string? q, [FromQuery] int limit = 10)
    {
        var result = await _mediator.Send(new SearchUsersQuery 
        { 
            SearchTerm = q, 
            Limit = limit > 50 ? 50 : limit // Max 50 results
        });
        
        return Ok(ApiResponse<List<SearchUserDto>>.Ok(result, $"Tìm thấy {result.Count} user"));
    }
}
