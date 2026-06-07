using TaksManagementAI.API.Domain;
using DomainTask = TaksManagementAI.API.Domain.Task;

namespace TaksManagementAI.API.Application;

public sealed record CreateTaskRequest(
    Guid UserId,
    string? Title,
    string? Description,
    DateOnly? DueDate);

public sealed class CreateTaskCmd
{
    private readonly ITaskRepository taskRepository;

    public CreateTaskCmd(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<Result<TaskResponse>> Handle(
        CreateTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var taskResult = DomainTask.Create(
            request.UserId,
            request.Title,
            request.Description,
            request.DueDate,
            DateTimeOffset.UtcNow);

        if (taskResult.IsFailure)
        {
            return Result<TaskResponse>.Failure(taskResult.Error ?? "Task could not be created.", taskResult.ErrorType);
        }

        await taskRepository.AddAsync(taskResult.Value!, cancellationToken);

        return Result<TaskResponse>.Success(TaskResponse.FromDomain(taskResult.Value!));
    }
}
