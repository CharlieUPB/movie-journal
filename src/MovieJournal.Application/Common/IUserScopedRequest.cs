namespace MovieJournal.Application.Common;

public interface IUserScopedRequest
{
    Guid UserId { get; }
}

