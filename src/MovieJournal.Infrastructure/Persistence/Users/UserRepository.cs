using MovieJournal.Application.Users;
using MovieJournal.Domain.Entities;
using MovieJournal.Infrastructure.Persistence.Connection;
using System.Data;
using System.Globalization;

namespace MovieJournal.Infrastructure.Persistence.Users;

public sealed class UserRepository(ISqlConnectionFactory sqlConnectionFactory)
    : IUserRepository
{
    public Task<User> CreateAsync(User user)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO users (
                id,
                display_name,
                email,
                password_hash,
                created_at,
                updated_at,
                is_deleted
            )
            VALUES (
                @id,
                @display_name,
                @email,
                @password_hash,
                @created_at,
                @updated_at,
                @is_deleted
            );
            """;

        AddParameter(command, "@id", user.Id.ToString());
        AddParameter(command, "@display_name", user.DisplayName);
        AddParameter(command, "@email", user.Email);
        AddParameter(command, "@password_hash", user.PasswordHash);
        AddParameter(command, "@created_at", user.CreatedAt.ToString("O"));
        AddParameter(command, "@updated_at", user.UpdatedAt?.ToString("O"));
        AddParameter(command, "@is_deleted", user.IsDeleted == true ? 1 : 0);

        command.ExecuteNonQuery();

        return Task.FromResult(user);
    }

    public Task<User?> GetByEmailAsync(string email)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                id,
                display_name,
                email,
                password_hash,
                created_at,
                updated_at,
                is_deleted
            FROM users
            WHERE email = @email
              AND COALESCE(is_deleted, 0) = 0
            LIMIT 1;
            """;

        AddParameter(command, "@email", NormalizeEmail(email));

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return Task.FromResult<User?>(null);
        }

        return Task.FromResult<User?>(MapToUser(reader));
    }

    public Task<User?> GetByIdAsync(Guid id)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                id,
                display_name,
                email,
                password_hash,
                created_at,
                updated_at,
                is_deleted
            FROM users
            WHERE id = @id
              AND COALESCE(is_deleted, 0) = 0
            LIMIT 1;
            """;

        AddParameter(command, "@id", id.ToString());

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return Task.FromResult<User?>(null);
        }

        return Task.FromResult<User?>(MapToUser(reader));
    }

    public Task<bool> ExistsByEmailAsync(string email)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT COUNT(1)
            FROM users
            WHERE email = @email
              AND COALESCE(is_deleted, 0) = 0;
            """;

        AddParameter(command, "@email", NormalizeEmail(email));

        return Task.FromResult(Convert.ToInt32(command.ExecuteScalar()) > 0);
    }

    private static void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }

    private static User MapToUser(IDataRecord reader)
    {
        return User.Rebuild(
            Guid.Parse(reader.GetString(reader.GetOrdinal("id"))),
            reader.GetString(reader.GetOrdinal("display_name")),
            reader.GetString(reader.GetOrdinal("email")),
            reader.GetString(reader.GetOrdinal("password_hash")),
            DateTime.Parse(
                reader.GetString(reader.GetOrdinal("created_at")),
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind),
            reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : DateTime.Parse(
                    reader.GetString(reader.GetOrdinal("updated_at")),
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind),
            !reader.IsDBNull(reader.GetOrdinal("is_deleted")) &&
            reader.GetInt32(reader.GetOrdinal("is_deleted")) == 1);
    }

    private static string NormalizeEmail(string email)
    {
        return email.Trim().ToLowerInvariant();
    }
}
