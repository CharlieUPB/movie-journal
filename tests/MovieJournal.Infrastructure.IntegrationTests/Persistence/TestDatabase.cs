using System.Data;
using Microsoft.Data.Sqlite;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.ValueObjects;
using MovieJournal.Infrastructure.Persistence.Connection;
using MovieJournal.Infrastructure.Persistence.Initializer;
using MovieJournal.Infrastructure.Persistence.MovieReviews;
using MovieJournal.Infrastructure.Persistence.ReviewComments;
using MovieJournal.Infrastructure.Persistence.Users;
using MovieJournal.Infrastructure.Security;

namespace MovieJournal.Infrastructure.IntegrationTests.Persistence;

internal sealed class TestDatabase : IDisposable
{
    public static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    public const string ValidReviewContent = "This review has enough content to satisfy the domain validation rules for repository integration testing.";
    public const string UpdatedReviewContent = "This updated review has enough content to satisfy the domain validation rules for repository integration testing.";

    private readonly SqliteConnectionFactory _connectionFactory;

    private TestDatabase(string databasePath)
    {
        DatabasePath = databasePath;
        _connectionFactory = new SqliteConnectionFactory($"Data Source={databasePath};Pooling=False");
        UserRepository = new UserRepository(_connectionFactory);
        Repository = new MovieReviewsRepository(_connectionFactory);
        ReviewCommentsRepository = new ReviewCommentsRepository(_connectionFactory);
        Initializer = new SqliteDatabaseInitializer(_connectionFactory, new PasswordHasher());
    }

    public string DatabasePath { get; }
    public UserRepository UserRepository { get; }
    public MovieReviewsRepository Repository { get; }
    public ReviewCommentsRepository ReviewCommentsRepository { get; }
    public SqliteDatabaseInitializer Initializer { get; }

    public static async Task<TestDatabase> CreateAsync(bool clearMovieReviewData = false)
    {
        var databasePath = Path.Combine(Path.GetTempPath(), $"movie-journal-{Guid.NewGuid():N}.db");
        var database = new TestDatabase(databasePath);

        await database.Initializer.InitializeAsync();

        if (clearMovieReviewData)
        {
            database.ClearMovieReviewData();
        }

        return database;
    }

    public static MovieReview CreateValidMovieReview(
        Guid? userId = null,
        string movieTitle = "Project Hail Mary",
        int? releaseYear = 2024,
        string reviewTitle = "Great story",
        string reviewContent = ValidReviewContent,
        int rating = 5)
    {
        var movieInformation = MovieInformation.Create(
            movieTitle,
            DateOnly.FromDateTime(DateTime.Today),
            releaseYear);

        var reviewInformation = ReviewInformation.Create(
            reviewTitle,
            reviewContent,
            rating);

        return MovieReview.Create(userId ?? DemoUserId, movieInformation, reviewInformation);
    }

    public static ReviewComment CreateValidReviewComment(
        Guid movieReviewId,
        Guid? userId = null,
        string content = "I agree with this review and enjoyed reading the author's thoughts.")
    {
        return ReviewComment.Create(
            movieReviewId,
            userId ?? DemoUserId,
            content);
    }

    public bool TableExists(string tableName)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT COUNT(1)
            FROM sqlite_master
            WHERE type = 'table'
              AND name = @table_name;
            """;

        AddParameter(command, "@table_name", tableName);

        return Convert.ToInt32(command.ExecuteScalar()) == 1;
    }

    public int CountRows(string tableName)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        using var command = connection.CreateCommand();

        command.CommandText = $"SELECT COUNT(1) FROM {tableName};";

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public int CountRows(string tableName, string columnName, object value)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        using var command = connection.CreateCommand();

        command.CommandText = $"SELECT COUNT(1) FROM {tableName} WHERE {columnName} = @value;";
        AddParameter(command, "@value", value);

        return Convert.ToInt32(command.ExecuteScalar());
    }

    public void InsertUser(Guid userId)
    {
        using var connection = _connectionFactory.CreateNewConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO users (
                id,
                display_name,
                email,
                password_hash,
                created_at,
                is_deleted
            )
            VALUES (
                @id,
                @display_name,
                @email,
                @password_hash,
                @created_at,
                0
            );
            """;

        AddParameter(command, "@id", userId.ToString());
        AddParameter(command, "@display_name", $"User {userId:N}");
        AddParameter(command, "@email", $"{userId:N}@moviejournal.test");
        AddParameter(command, "@password_hash", "test-password-hash");
        AddParameter(command, "@created_at", DateTime.UtcNow.ToString("O"));

        command.ExecuteNonQuery();
    }

    private void ClearMovieReviewData()
    {
        using var connection = _connectionFactory.CreateNewConnection();

        Execute(connection, "DELETE FROM review_comments;");
        Execute(connection, "DELETE FROM movie_reviews;");
    }

    private static void Execute(IDbConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void AddParameter(IDbCommand command, string name, object value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value;
        command.Parameters.Add(parameter);
    }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        if (File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
        }
    }
}
