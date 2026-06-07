namespace MovieJournal.Infrastructure.IntegrationTests.Persistence;

public class SqliteDatabaseInitializerTests
{
    [Fact]
    public async Task InitializeAsync_ShouldCreateRequiredTables()
    {
        using var database = await TestDatabase.CreateAsync();

        Assert.True(database.TableExists("users"));
        Assert.True(database.TableExists("movie_reviews"));
        Assert.True(database.TableExists("review_comments"));
    }

    [Fact]
    public async Task InitializeAsync_ShouldSeedDemoUserAndReviews()
    {
        using var database = await TestDatabase.CreateAsync();

        Assert.Equal(1, database.CountRows("users", "id", TestDatabase.DemoUserId.ToString()));
        Assert.True(database.CountRows("movie_reviews") > 0);
    }

    [Fact]
    public async Task InitializeAsync_WhenCalledTwice_ShouldNotDuplicateSeedData()
    {
        using var database = await TestDatabase.CreateAsync();
        var usersBeforeSecondInitialization = database.CountRows("users");

        await database.Initializer.InitializeAsync();

        Assert.Equal(usersBeforeSecondInitialization, database.CountRows("users"));
        Assert.Equal(1, database.CountRows("users", "id", TestDatabase.DemoUserId.ToString()));
    }
}
