using System.Collections.Concurrent;
using TaksManagementAI.API.Application;
using DomainTask = TaksManagementAI.API.Domain.Task;

namespace TaksManagementAI.API.Infrastructure;

public sealed class TaskRepository : ITaskRepository
{
    private readonly ConcurrentDictionary<Guid, DomainTask> tasks = new();

    public Task AddAsync(DomainTask task, CancellationToken cancellationToken = default)
    {
        tasks[task.Id] = task;

        return Task.CompletedTask;
    }

    public Task<DomainTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        tasks.TryGetValue(id, out var task);

        return Task.FromResult(task);
    }

    public Task<IReadOnlyList<DomainTask>> ListAsync(Guid? userId, CancellationToken cancellationToken = default)
    {
        IEnumerable<DomainTask> query = tasks.Values;

        if (userId.HasValue)
        {
            query = query.Where(task => task.UserId == userId.Value);
        }

        var result = query
            .OrderBy(task => task.DueDate ?? DateOnly.MaxValue)
            .ThenBy(task => task.CreatedAt)
            .ToList();

        return Task.FromResult<IReadOnlyList<DomainTask>>(result);
    }

    public Task UpdateAsync(DomainTask task, CancellationToken cancellationToken = default)
    {
        tasks[task.Id] = task;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        tasks.TryRemove(id, out _);

        return Task.CompletedTask;
    }
}
