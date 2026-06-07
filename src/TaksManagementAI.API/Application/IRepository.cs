using DomainTask = TaksManagementAI.API.Domain.Task;

namespace TaksManagementAI.API.Application;

public interface ITaskRepository
{
    Task AddAsync(DomainTask task, CancellationToken cancellationToken = default);

    Task<DomainTask?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<DomainTask>> ListAsync(Guid? userId, CancellationToken cancellationToken = default);

    Task UpdateAsync(DomainTask task, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
