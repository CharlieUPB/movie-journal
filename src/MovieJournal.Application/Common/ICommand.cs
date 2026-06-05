namespace MovieJournal.Application.Common;

public interface ICommand<T, K> where T: IUserScopedRequest
{
    Task<K> Execute(T request);
}

