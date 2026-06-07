
namespace MovieJournal.Application.Exceptions;

public class UseCaseException : Exception
{
    public UseCaseException(string message) : base(message) { }
}
