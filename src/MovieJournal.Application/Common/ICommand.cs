namespace MovieJournal.Application.Common;

public interface ICommand<TRequest, TResponse> where TRequest: IUserScopedRequest
{
    Task<TResponse> Execute(TRequest request);
}

public interface ICommand<TRequest>
{
    Task Execute(TRequest request);
}
