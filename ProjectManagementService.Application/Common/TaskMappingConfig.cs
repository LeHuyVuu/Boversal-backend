using Mapster;
using ProjectManagementService.Application.DTOs.Task;
using ProjectManagementService.Domain.Entities;
using TaskEntity = ProjectManagementService.Domain.Entities.Task;

namespace ProjectManagementService.Application.Common;

/// <summary>
/// Mapster mapping configuration cho Task
/// </summary>
public class TaskMappingConfig : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        // Task entity -> TaskDto
        config.NewConfig<TaskEntity, TaskDto>()
            .Map(dest => dest.StatusName, src => src.Status != null ? src.Status.Name : null)
            .Map(dest => dest.StatusColor, src => src.Status != null ? src.Status.Color : null)
            .Map(dest => dest.CreatedByName, src => src.CreatedByNavigation != null ? src.CreatedByNavigation.FullName : null)
            .Map(dest => dest.Assignees, src => src.TaskAssignees.Select(ta => new TaskAssigneeDto
            {
                UserId = ta.UserId,
                FullName = ta.User.FullName,
                Email = ta.User.Email,
                AvatarUrl = ta.User.AvatarUrl
            }).ToList());

        // CreateTaskCommand -> Task entity
        config.NewConfig<Features.Tasks.Commands.CreateTaskCommand, TaskEntity>()
            .Ignore(dest => dest.Id)
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.DeletedAt)
            .Ignore(dest => dest.CreatedByNavigation)
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.TaskAssignees);

        // UpdateTaskCommand -> Task entity
        config.NewConfig<Features.Tasks.Commands.UpdateTaskCommand, TaskEntity>()
            .Ignore(dest => dest.CreatedBy)
            .Ignore(dest => dest.CreatedAt)
            .Ignore(dest => dest.UpdatedAt)
            .Ignore(dest => dest.DeletedAt)
            .Ignore(dest => dest.CreatedByNavigation)
            .Ignore(dest => dest.Project)
            .Ignore(dest => dest.Status)
            .Ignore(dest => dest.TaskAssignees);

        // PatchTaskCommand -> PatchTaskDto
        config.NewConfig<Features.Tasks.Commands.PatchTaskCommand, PatchTaskDto>();
    }
}
