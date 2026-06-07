using TaksManagementAI.API.Domain;

namespace TaksManagementAI.API.Application;

public sealed record GetTaskRequest(Guid Id);

public sealed record ListTasksRequest(Guid? UserId);

public sealed class GetTaskQuery
{
    private readonly ITaskRepository taskRepository;

    public GetTaskQuery(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<Result<TaskResponse>> Handle(
        GetTaskRequest request,
        CancellationToken cancellationToken = default)
    {
        var task = await taskRepository.GetByIdAsync(request.Id, cancellationToken);
        if (task is null)
        {
            return Result<TaskResponse>.Failure("Task was not found.", ErrorType.NotFound);
        }

        return Result<TaskResponse>.Success(TaskResponse.FromDomain(task));
    }
}

public sealed class ListTasksQuery
{
    private readonly ITaskRepository taskRepository;

    public ListTasksQuery(ITaskRepository taskRepository)
    {
        this.taskRepository = taskRepository;
    }

    public async System.Threading.Tasks.Task<Result<IReadOnlyList<TaskResponse>>> Handle(
        ListTasksRequest request,
        CancellationToken cancellationToken = default)
    {
        var tasks = await taskRepository.ListAsync(request.UserId, cancellationToken);
        var response = tasks.Select(TaskResponse.FromDomain).ToList();

        return Result<IReadOnlyList<TaskResponse>>.Success(response);
    }
}
