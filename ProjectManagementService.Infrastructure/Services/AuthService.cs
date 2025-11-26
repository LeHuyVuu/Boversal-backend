using Mapster;
using Microsoft.AspNetCore.Identity;
using ProjectManagementService.Application.DTOs.Auth;
using ProjectManagementService.Application.Interfaces;
using ProjectManagementService.Domain.Entities;
using ProjectManagementService.Domain.Exceptions;

namespace ProjectManagementService.Infrastructure.Services;

/// <summary>
/// Service xử lý authentication (đăng nhập, đăng ký, lấy user hiện tại)
/// Implements IAuthService (Interface ở Application layer)
/// Dependencies:
/// - IUserRepository: Query/Save user từ database
/// - IJwtService: Tạo và validate JWT token
/// - ICurrentUserService: Lấy UserId từ HttpContext (JWT claims)
/// - IPasswordHasher: Hash và verify password
/// </summary>
public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IJwtService _jwtService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IPasswordHasher<User> _passwordHasher;

    public AuthService(
        IUserRepository userRepository,
        IJwtService jwtService,
        ICurrentUserService currentUserService,
        IPasswordHasher<User> passwordHasher)
    {
        _userRepository = userRepository;
        _jwtService = jwtService;
        _currentUserService = currentUserService;
        _passwordHasher = passwordHasher;
    }

    /// <summary>
    /// Xử lý đăng nhập
    /// 1. Tìm user trong database theo email
    /// 2. Verify password bằng PasswordHasher
    /// 3. Tạo JWT token chứa UserId, Email trong Claims
    /// 4. Trả về token + thông tin user
    /// </summary>
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Tìm user theo email
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
            throw new NotFoundException("Email hoặc mật khẩu không đúng");

        // Verify password bằng ASP.NET Identity PasswordHasher
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginDto.Password);
        if (result == PasswordVerificationResult.Failed)
            throw new ValidationException("Email hoặc mật khẩu không đúng");

        // Tạo JWT token (chứa UserId, Email trong Claims)
        var token = _jwtService.GenerateToken(user);

        // Trả về response
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    /// <summary>
    /// Xử lý đăng ký tài khoản mới
    /// 1. Check email đã tồn tại chưa
    /// 2. Tạo User entity từ RegisterDto (dùng Mapster)
    /// 3. Hash password
    /// 4. Lưu vào database
    /// 5. Tạo JWT token
    /// 6. Trả về token + thông tin user
    /// </summary>
    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check email đã tồn tại chưa
        if (await _userRepository.EmailExistsAsync(registerDto.Email))
            throw new ConflictException("Email đã được đăng ký");

        // Dùng Mapster map RegisterDto -> User
        var user = registerDto.Adapt<User>();
        user.Username = registerDto.Email; // Set username = email
        user.CreatedAt = DateTime.UtcNow;
        user.UpdatedAt = DateTime.UtcNow;

        // Hash password bằng ASP.NET Identity PasswordHasher
        user.PasswordHash = _passwordHasher.HashPassword(user, registerDto.Password);

        // Lưu vào database
        user = await _userRepository.AddAsync(user);

        // Tạo JWT token
        var token = _jwtService.GenerateToken(user);

        // Trả về response
        return new AuthResponseDto
        {
            UserId = user.Id,
            Email = user.Email,
            FullName = user.FullName,
            Token = token,
            ExpiresAt = DateTime.UtcNow.AddHours(24)
        };
    }

    /// <summary>
    /// Lấy thông tin user hiện tại từ JWT token
    /// CƠ CHẾ:
    /// 1. CurrentUserService.UserId đọc Claims từ HttpContext.User (đã được set bởi JWT middleware)
    /// 2. Query database lấy thông tin user đầy đủ
    /// 3. Map User entity -> UserDto bằng Mapster
    /// 4. Trả về UserDto
    /// </summary>
    public async Task<UserDto?> GetCurrentUserAsync()
    {
        // Lấy UserId từ JWT claims (CurrentUserService tự động đọc từ HttpContext)
        var userId = _currentUserService.UserId;
        if (userId == null)
            return null;

        // Query database lấy thông tin user
        var user = await _userRepository.GetByIdAsync(userId.Value);
        if (user == null)
            return null;

        // Dùng Mapster map User -> UserDto
        return user.Adapt<UserDto>();
    }

    public string HashPassword(string password)
    {
        return _passwordHasher.HashPassword(null!, password);
    }

    public bool VerifyPassword(string password, string hashedPassword)
    {
        var result = _passwordHasher.VerifyHashedPassword(null!, hashedPassword, password);
        return result == PasswordVerificationResult.Success;
    }
}
