using MovieJournal.Infrastructure.Security;

namespace MovieJournal.Infrastructure.IntegrationTests.Security;

public class PasswordHasherTests
{
    [Fact]
    public void PasswordHasher_ShouldVerifyCorrectPasswordAndRejectWrongPassword()
    {
        var passwordHasher = new PasswordHasher();

        var passwordHash = passwordHasher.HashPassword("Demo123!");

        Assert.True(passwordHasher.VerifyPassword("Demo123!", passwordHash));
        Assert.False(passwordHasher.VerifyPassword("WrongPassword", passwordHash));
    }
}
