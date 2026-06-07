using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.UnitTests.Entities;

public class UserTests
{
    [Fact]
    public void Create_ShouldSaveDisplayNameNormalizedEmailAndPasswordHash()
    {
        var user = User.Create(" Demo User ", " DEMO@MovieJournal.COM ", "hashed-password");

        Assert.Equal("Demo User", user.DisplayName);
        Assert.Equal("demo@moviejournal.com", user.Email);
        Assert.Equal("hashed-password", user.PasswordHash);
    }

    [Theory]
    [InlineData(null, "demo@moviejournal.com", "hashed-password", "Display name is required")]
    [InlineData("  ", "demo@moviejournal.com", "hashed-password", "Display name is required")]
    [InlineData("Demo User", null, "hashed-password", "Email is required")]
    [InlineData("Demo User", "  ", "hashed-password", "Email is required")]
    [InlineData("Demo User", "demo@moviejournal.com", null, "Password hash is required")]
    [InlineData("Demo User", "demo@moviejournal.com", "  ", "Password hash is required")]
    public void Create_WhenRequiredFieldsAreInvalid_ShouldThrowDomainException(
        string? displayName,
        string? email,
        string? passwordHash,
        string expectedMessage)
    {
        var exception = Assert.Throws<DomainException>(() =>
            User.Create(displayName, email, passwordHash));

        Assert.Equal(expectedMessage, exception.Message);
    }
}
