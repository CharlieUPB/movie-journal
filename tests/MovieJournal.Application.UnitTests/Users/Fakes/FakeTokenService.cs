using MovieJournal.Application.Security;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.UnitTests.Users.Fakes;

internal class FakeTokenService : ITokenService
{
    public string CreateToken(User user)
    {
        return $"token:{user.Id}";
    }
}
