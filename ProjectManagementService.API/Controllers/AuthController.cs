using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProjectManagementService.Application.Common;
using ProjectManagementService.Application.DTOs.Auth;
using ProjectManagementService.Application.Features.Auth.Commands;
using ProjectManagementService.Application.Features.Auth.Queries;

namespace ProjectManagementService.API.Controllers;

/// <summary>
/// Controller quản lý Authentication (Login, Register, GetCurrentUser)
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Đăng nhập bằng Email và Password
    /// Token được lưu vào HTTP-Only Cookie tự động
    /// </summary>
    /// <param name="command">Thông tin đăng nhập (Email, Password)</param>
    /// <returns>Thông tin user (KHÔNG có token trong response)</returns>
    /// <response code="200">Đăng nhập thành công, token lưu vào cookie</response>
    /// <response code="400">Email hoặc password không đúng</response>
    [HttpPost("login")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        
        // Lưu JWT token vào HTTP-Only Cookie (bảo mật, tự động gửi kèm mọi request)
        Response.Cookies.Append("jwt", result.Token, new CookieOptions
        {
            HttpOnly = true,        // Cookie chỉ được truy cập từ server, không từ JavaScript (chống XSS)
            Secure = true,          // Chỉ gửi qua HTTPS (production)
            SameSite = SameSiteMode.None, // Allow cross-site (Gateway port khác backend port)
            Expires = result.ExpiresAt      // Hết hạn sau 24h
        });
        
        // Trả về thông tin user KHÔNG có token
        var userInfo = new UserDto
        {
            Id = result.UserId,
            Email = result.Email,
            FullName = result.FullName,
            PhoneNumber = null // Sẽ lấy đầy đủ từ GetCurrentUser nếu cần
        };
        
        return Ok(ApiResponse<UserDto>.Ok(userInfo, "Đăng nhập thành công"));
    }

    /// <summary>
    /// Đăng ký tài khoản mới
    /// Token được lưu vào HTTP-Only Cookie tự động
    /// </summary>
    /// <param name="command">Thông tin đăng ký (Email, Password, FullName, PhoneNumber)</param>
    /// <returns>Thông tin user mới (KHÔNG có token trong response)</returns>
    /// <response code="200">Tạo tài khoản thành công, token lưu vào cookie</response>
    /// <response code="409">Email đã được đăng ký</response>
    [HttpPost("register")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var result = await _mediator.Send(command);
        
        // Lưu JWT token vào HTTP-Only Cookie
        Response.Cookies.Append("jwt", result.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.None, // Allow cross-site (Gateway port khác backend port)
            Expires = result.ExpiresAt
        });
        
        // Trả về thông tin user KHÔNG có token
        var userInfo = new UserDto
        {
            Id = result.UserId,
            Email = result.Email,
            FullName = result.FullName,
            PhoneNumber = null
        };
        
        return Ok(ApiResponse<UserDto>.Ok(userInfo, "Đăng ký thành công"));
    }

    /// <summary>
    /// Lấy thông tin user hiện tại từ JWT token trong Cookie
    /// CƠ CHẾ: JWT token tự động được gửi từ Cookie
    /// -> ASP.NET Core Middleware đọc token từ Cookie
    /// -> Validate token
    /// -> Đọc Claims (UserId, Email) từ token
    /// -> CurrentUserService lấy UserId từ HttpContext.User
    /// -> Query database lấy thông tin user
    /// </summary>
    /// <returns>Thông tin user hiện tại</returns>
    /// <response code="200">Trả về thông tin user</response>
    /// <response code="401">Không có JWT token hoặc token không hợp lệ</response>
    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(typeof(ApiResponse<UserDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetCurrentUser()
    {
        var result = await _mediator.Send(new GetCurrentUserQuery());
        return Ok(ApiResponse<UserDto>.Ok(result, "Lấy thông tin thành công"));
    }

    /// <summary>
    /// Đăng xuất - Xóa JWT token khỏi cookie
    /// </summary>
    /// <returns>Kết quả đăng xuất</returns>
    /// <response code="200">Đăng xuất thành công</response>
    [HttpPost("logout")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    public IActionResult Logout()
    {
        // Xóa JWT cookie
        Response.Cookies.Delete("jwt");
        return Ok(ApiResponse.Ok("Đăng xuất thành công"));
    }
}
