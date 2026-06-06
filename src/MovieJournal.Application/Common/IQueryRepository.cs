using MovieJournal.Domain.Common;

namespace MovieJournal.Application.Common;

public interface IQueryRepository<T> where T : AuditableEntity
{
    Task<T> GetByIdAsync(Guid id);

    Task<IReadOnlyList<T>> GetAllAsync();
}

