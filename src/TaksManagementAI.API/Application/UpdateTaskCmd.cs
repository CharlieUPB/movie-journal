using TaksManagementAI.API.Domain;
using TaksManagementAI.API.Validator;

namespace TaksManagementAI.API.Application;

public sealed record UpdateTaskRequest(
    Guid UserId,
    string? Title,
    string? Description,
    string? Status,
    DateOnly? DueDate);

public sealed class UpdateTaskCmd
{
    private readonly ITaskRepository taskRepository;

    public UpdateTaskCmd(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<Result<TaskResponse>> Handle(
        Guid id,
        UpdateTaskRequest request,
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

        var updateResult = task.Update(
            request.UserId,
            request.Title,
            request.Description,
            statusResult.Value,
            request.DueDate,
            DateOnly.FromDateTime(DateTime.UtcNow),
            DateTimeOffset.UtcNow);

        if (updateResult.IsFailure)
        {
            return Result<TaskResponse>.Failure(updateResult.Error ?? "Task could not be updated.", updateResult.ErrorType);
        }

        await taskRepository.UpdateAsync(task, cancellationToken);

        return Result<TaskResponse>.Success(TaskResponse.FromDomain(task));
    }
}
