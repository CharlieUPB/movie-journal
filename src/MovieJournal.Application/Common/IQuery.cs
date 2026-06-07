namespace MovieJournal.Application.Common;

public interface IQuery<TRequest, TResponse>
{
    Task<TResponse> Execute(TRequest request);
}