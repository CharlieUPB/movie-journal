using MovieJournal.Application.Security;

namespace MovieJournal.Application.UnitTests.Users.Fakes;

internal class FakePasswordHasher : IPasswordHasher
{
    public string HashPassword(string password)
    {
        return $"hashed:{password}";
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return passwordHash == HashPassword(password);
    }
}
