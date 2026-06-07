using TaksManagementAI.API.Domain;

namespace TaksManagementAI.API.Application;

public sealed record DeleteTaskRequest(Guid UserId);

public sealed class DeleteTaskCmd
{
    private readonly ITaskRepository taskRepository;

    public DeleteTaskCmd(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<Result> Handle(
        Guid id,
        DeleteTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request.UserId == Guid.Empty)
        {
            return Result.Failure("userId is required.");
        }

        var task = await taskRepository.GetByIdAsync(id, cancellationToken);
        if (task is not null && task.UserId != request.UserId)
        {
            return Result.Failure("Only the task owner can delete this task.", ErrorType.Forbidden);
        }

        await taskRepository.DeleteAsync(id, cancellationToken);

        return Result.Success();
    }
}
