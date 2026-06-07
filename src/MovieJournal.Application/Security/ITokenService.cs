using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.Security;

public interface ITokenService
{
    string CreateToken(User user);
}
