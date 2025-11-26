using MediatR;
using ProjectManagementService.Application.Interfaces;

namespace ProjectManagementService.Application.Features.Tasks.Commands;

/// <summary>
/// Handler xóa task (soft delete)
/// </summary>
public class DeleteTaskCommandHandler : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly ITaskRepository _taskRepository;

    public DeleteTaskCommandHandler(ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<bool> Handle(DeleteTaskCommand request, CancellationToken cancellationToken)
    {
        return await _taskRepository.DeleteAsync(request.Id);
    }
}
