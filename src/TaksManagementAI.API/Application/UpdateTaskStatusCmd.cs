using TaksManagementAI.API.Domain;
using TaksManagementAI.API.Validator;

namespace TaksManagementAI.API.Application;

public sealed record UpdateTaskStatusRequest(Guid UserId, string? Status);

public sealed class UpdateTaskStatusCmd
{
    private readonly ITaskRepository taskRepository;

    public UpdateTaskStatusCmd(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<Result<TaskResponse>> Handle(
        Guid id,
        UpdateTaskStatusRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is null)
        {
            return Result<TaskResponse>.Failure("Task was not found.", ErrorType.NotFound);
        }

        var statusResult = TaskValidator.ParseStatus(request.Status);
        if (statusResult.IsFailure)
        {
            return Result<TaskResponse>.Failure(statusResult.Error ?? "Invalid status.", statusResult.ErrorType);
        }

        var updateResult = task.ChangeStatus(
            request.UserId,
            statusResult.Value,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateTimeOffset.UtcNow);

        if (updateResult.IsFailure)
        {
            return Result<TaskResponse>.Failure(updateResult.Error ?? "Task status could not be updated.", updateResult.ErrorType);
        }

        await taskRepository.UpdateAsync(task, cancellationToken);

        return Result<TaskResponse>.Success(TaskResponse.FromDomain(task));
    }
}
