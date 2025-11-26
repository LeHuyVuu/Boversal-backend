using Mapster;
using ProjectManagementService.Application.DTOs;
using ProjectManagementService.Application.DTOs.Auth;
using ProjectManagementService.Application.Features.Auth.Commands;
using ProjectManagementService.Application.Features.Projects.Commands;
using ProjectManagementService.Domain.Entities;

namespace ProjectManagementService.Application.Common;

// === CÁCH 1: Convention-based (tự động mapping theo tên property) ===
// Mapster tự động map các property có cùng tên
// Chỉ cần config những trường hợp đặc biệt

public class ProjectMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Project -> ProjectDto (auto-map hết, không cần config gì thêm)
        config.NewConfig<Project, ProjectDto>();

        // CreateProjectCommand -> Project
        config.NewConfig<CreateProjectCommand, Project>()
            .Map(dest => dest.Status, src => "active")  // Set default
            .Map(dest => dest.CreatedAt, src => DateTime.UtcNow)
            .Ignore(dest => dest.Id)  // Bỏ qua Id khi tạo mới
            .Ignore(dest => dest.UpdatedAt);

        // UpdateProjectCommand -> Project
        config.NewConfig<UpdateProjectCommand, Project>()
            .Map(dest => dest.UpdatedAt, src => DateTime.UtcNow)
            .IgnoreNullValues(true)  // Chỉ update field có giá trị
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.OwnerId);


        config.NewConfig<LoginCommand, LoginDto>();


        config.NewConfig<RegisterCommand, RegisterDto>();
    }
}

public class UserMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // User -> UserDto
        config.NewConfig<User, UserDto>()
            .Map(dest => dest.PhoneNumber, src => src.Phone);  // Chỉ config field khác tên
    }
}

// === CÁCH 2: Global Settings (áp dụng cho TẤT CẢ mappings) ===
public static class MappingConfig
{
    public static void ConfigureGlobalSettings(TypeAdapterConfig config)
    {
        // Tự động bỏ qua null values khi update
        config.Default.IgnoreNullValues(true);
        
        // Tự động map enum sang string
        config.Default.MapToConstructor(true);
        
        // Tự động map theo tên không phân biệt hoa thường
        config.Default.NameMatchingStrategy(NameMatchingStrategy.Flexible);
    }
}
