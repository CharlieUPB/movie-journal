using MovieJournal.Application.Security;
using MovieJournal.Domain.Enums;
using MovieJournal.Infrastructure.Persistence.Connection;
using System.Data;

namespace MovieJournal.Infrastructure.Persistence.Initializer;

public class SqliteDatabaseInitializer : IDatabaseInitializer
{
    private readonly ISqlConnectionFactory _connectionFactory;
    private readonly IPasswordHasher _passwordHasher;

    public SqliteDatabaseInitializer(
        ISqlConnectionFactory sqlConnectionFactory,
        IPasswordHasher passwordHasher)
    {
        _connectionFactory = sqlConnectionFactory;
        _passwordHasher = passwordHasher;
    }

    public Task InitializeAsync()
    {
        using var connection = _connectionFactory.GetOpenConnection();

        CreateTables(connection);
        SeedData(connection, _passwordHasher);

        return Task.CompletedTask;
    }

    private static void CreateTables(IDbConnection connection)
    {
        Execute(connection, """
            CREATE TABLE IF NOT EXISTS users (
                id TEXT PRIMARY KEY,
                display_name TEXT NOT NULL,
                email TEXT NOT NULL UNIQUE,
                password_hash TEXT NOT NULL,
                created_at TEXT NOT NULL,
                updated_at TEXT NULL,
                is_deleted INTEGER NOT NULL DEFAULT 0
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS movie_reviews (
                id TEXT PRIMARY KEY,
                user_id TEXT NOT NULL,
                movie_title TEXT NOT NULL,
                release_year INTEGER NULL,
                review_title TEXT NOT NULL,
                review_content TEXT NOT NULL,
                rating INTEGER NOT NULL,
                status INTEGER NOT NULL,
                created_at TEXT NOT NULL,
                updated_at TEXT NULL,
                is_deleted INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (user_id) REFERENCES users(id)
            );
            """);

        Execute(connection, """
            CREATE TABLE IF NOT EXISTS review_comments (
                id TEXT PRIMARY KEY,
                movie_review_id TEXT NOT NULL,
                user_id TEXT NOT NULL,
                content TEXT NOT NULL,
                created_at TEXT NOT NULL,
                updated_at TEXT NULL,
                is_deleted INTEGER NOT NULL DEFAULT 0,
                FOREIGN KEY (movie_review_id) REFERENCES movie_reviews(id),
                FOREIGN KEY (user_id) REFERENCES users(id)
            );
            """);
    }

    private static void SeedData(IDbConnection connection, IPasswordHasher passwordHasher)
    {
        if (HasData(connection, "users"))
        {
            return;
        }

        var now = DateTime.UtcNow.ToString("O");
        var demoUserId = "11111111-1111-1111-1111-111111111111";
        var demoPasswordHash = passwordHasher.HashPassword("Demo123!");

        Execute(connection, $"""
            INSERT INTO users (
                id,
                display_name,
                email,
                password_hash,
                created_at,
                is_deleted
            )
            VALUES
            (
                '{demoUserId}',
                'Demo User',
                'demo@moviejournal.com',
                '{demoPasswordHash}',
                '{now}',
                0
            );
            """);

        Execute(connection, $"""
            INSERT INTO movie_reviews (
                id,
                user_id,
                movie_title,
                release_year,
                review_title,
                review_content,
                rating,
                status,
                created_at,
                is_deleted
            )
            VALUES
            (
                'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
                '{demoUserId}',
                'Dune: Part Two',
                2024,
                'One of the greatest sci-fi movies',
                'Denis Villeneuve created a visually stunning and emotionally powerful sci-fi experience with incredible performances, beautiful cinematography, and a story that feels epic without losing its human side.',
                5,
                {(int)ReviewStatus.Published},
                '{now}',
                0
            ),
            (
                'bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb',
                '{demoUserId}',
                'Project Hail Mary',
                NULL,
                'Excited for this adaptation',
                'This is a draft review for a movie adaptation I am really looking forward to watching in the future. The story has a lot of heart, science, and adventure.',
                5,
                {(int)ReviewStatus.Draft},
                '{now}',
                0
            ),
            (
                'cccccccc-cccc-cccc-cccc-cccccccccccc',
                '{demoUserId}',
                'Interstellar',
                2014,
                'Emotional and ambitious sci-fi',
                'A beautiful science fiction movie that combines emotional family drama with ambitious concepts about time, space, sacrifice, and hope. It remains memorable after watching it.',
                5,
                {(int)ReviewStatus.Published},
                '{now}',
                0
            );
            """);

        Execute(connection, $"""
            INSERT INTO review_comments (
                id,
                movie_review_id,
                user_id,
                content,
                created_at,
                is_deleted
            )
            VALUES
            (
                'dddddddd-dddd-dddd-dddd-dddddddddddd',
                'aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa',
                '{demoUserId}',
                'I agree. The visuals and the soundtrack made the movie feel huge, but the characters still felt personal and emotional.',
                '{now}',
                0
            );
            """);
    }

    private static bool HasData(IDbConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();

        command.CommandText = $"SELECT COUNT(1) FROM {tableName};";

        var result = command.ExecuteScalar();

        return Convert.ToInt32(result) > 0;
    }

    private static void Execute(IDbConnection connection, string sql)
    {
        using var command = connection.CreateCommand();

        command.CommandText = sql;

        command.ExecuteNonQuery();
    }
}
