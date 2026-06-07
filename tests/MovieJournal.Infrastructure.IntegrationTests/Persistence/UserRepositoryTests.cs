using MovieJournal.Domain.Entities;

namespace MovieJournal.Infrastructure.IntegrationTests.Persistence;

public class UserRepositoryTests
{
    [Fact]
    public async Task CreateAsyncAndGetByEmailAsync_ShouldPersistAndRetrieveUser()
    {
        using var database = await TestDatabase.CreateAsync();
        var email = $"{Guid.NewGuid():N}@moviejournal.test";
        var user = User.Create("Test User", email, "hashed-password");

        await database.UserRepository.CreateAsync(user);

        var savedUser = await database.UserRepository.GetByEmailAsync(email.ToUpperInvariant());

        Assert.NotNull(savedUser);
        Assert.Equal(user.Id, savedUser.Id);
        Assert.Equal("Test User", savedUser.DisplayName);
        Assert.Equal(email, savedUser.Email);
        Assert.Equal("hashed-password", savedUser.PasswordHash);
    }

    [Fact]
    public async Task ExistsByEmailAsync_ShouldReturnTrueForExistingEmailAndFalseForMissingEmail()
    {
        using var database = await TestDatabase.CreateAsync();
        var email = $"{Guid.NewGuid():N}@moviejournal.test";
        var user = User.Create("Test User", email, "hashed-password");
        await database.UserRepository.CreateAsync(user);

        var exists = await database.UserRepository.ExistsByEmailAsync(email.ToUpperInvariant());
        var missingExists = await database.UserRepository.ExistsByEmailAsync($"{Guid.NewGuid():N}@moviejournal.test");

        Assert.True(exists);
        Assert.False(missingExists);
    }
}
