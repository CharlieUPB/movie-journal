using MovieJournal.Domain.Common;

namespace MovieJournal.Application.Common;

public interface ICommandRepository<T> where T : AuditableEntity
{
    Task<T> CreateAsync(T entity);
    Task<T> UpdateAync(T updatedEntity);
    Task<T> DeleteAsync(T entity);
}

