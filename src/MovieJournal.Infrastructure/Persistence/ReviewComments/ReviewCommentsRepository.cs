using MovieJournal.Application.ReviewComments;
using MovieJournal.Domain.Entities;
using MovieJournal.Infrastructure.Persistence.Connection;
using System.Data;
using System.Globalization;

namespace MovieJournal.Infrastructure.Persistence.ReviewComments;

public sealed class ReviewCommentsRepository(ISqlConnectionFactory sqlConnectionFactory)
    : IReviewCommentsRepository
{
    public Task<ReviewComment> CreateAsync(ReviewComment entity)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            INSERT INTO review_comments (
                id,
                movie_review_id,
                user_id,
                content,
                created_at,
                updated_at,
                is_deleted
            )
            VALUES (
                @id,
                @movie_review_id,
                @user_id,
                @content,
                @created_at,
                @updated_at,
                @is_deleted
            );
            """;

        AddParameter(command, "@id", entity.Id.ToString());
        AddParameter(command, "@movie_review_id", entity.MovieReviewId.ToString());
        AddParameter(command, "@user_id", entity.UserId.ToString());
        AddParameter(command, "@content", entity.Content);
        AddParameter(command, "@created_at", entity.CreatedAt.ToString("O"));
        AddParameter(command, "@updated_at", entity.UpdatedAt?.ToString("O"));
        AddParameter(command, "@is_deleted", entity.IsDeleted == true ? 1 : 0);

        command.ExecuteNonQuery();

        return Task.FromResult(entity);
    }

    public Task<ReviewComment?> GetByIdAsync(Guid id)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                id,
                movie_review_id,
                user_id,
                content,
                created_at,
                updated_at,
                is_deleted
            FROM review_comments
            WHERE id = @id
              AND COALESCE(is_deleted, 0) = 0
            LIMIT 1;
            """;

        AddParameter(command, "@id", id.ToString());

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return Task.FromResult<ReviewComment?>(null);
        }

        var reviewComment = MapToReviewComment(reader);

        return Task.FromResult<ReviewComment?>(reviewComment);
    }

    public Task<IReadOnlyList<ReviewComment>> GetByMovieReviewIdAsync(Guid movieReviewId)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                id,
                movie_review_id,
                user_id,
                content,
                created_at,
                updated_at,
                is_deleted
            FROM review_comments
            WHERE movie_review_id = @movie_review_id
              AND COALESCE(is_deleted, 0) = 0
            ORDER BY created_at ASC;
            """;

        AddParameter(command, "@movie_review_id", movieReviewId.ToString());

        using var reader = command.ExecuteReader();

        var reviewComments = new List<ReviewComment>();

        while (reader.Read())
        {
            reviewComments.Add(MapToReviewComment(reader));
        }

        return Task.FromResult<IReadOnlyList<ReviewComment>>(reviewComments);
    }

    public Task<ReviewComment> UpdateAsync(ReviewComment entity)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            UPDATE review_comments
            SET
                content = @content,
                updated_at = @updated_at,
                is_deleted = @is_deleted
            WHERE id = @id;
            """;

        AddParameter(command, "@id", entity.Id.ToString());
        AddParameter(command, "@content", entity.Content);
        AddParameter(command, "@updated_at", entity.UpdatedAt?.ToString("O"));
        AddParameter(command, "@is_deleted", entity.IsDeleted == true ? 1 : 0);

        command.ExecuteNonQuery();

        return Task.FromResult(entity);
    }

    public Task<ReviewComment> DeleteAsync(ReviewComment entity)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            UPDATE review_comments
            SET
                is_deleted = @is_deleted,
                updated_at = @updated_at
            WHERE id = @id;
            """;

        AddParameter(command, "@id", entity.Id.ToString());
        AddParameter(command, "@is_deleted", entity.IsDeleted == true ? 1 : 0);
        AddParameter(command, "@updated_at", entity.UpdatedAt?.ToString("O"));

        command.ExecuteNonQuery();

        return Task.FromResult(entity);
    }

    private static void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }

    private static ReviewComment MapToReviewComment(IDataRecord reader)
    {
        return ReviewComment.Rebuild(
            Guid.Parse(reader.GetString(reader.GetOrdinal("id"))),
            Guid.Parse(reader.GetString(reader.GetOrdinal("movie_review_id"))),
            Guid.Parse(reader.GetString(reader.GetOrdinal("user_id"))),
            reader.GetString(reader.GetOrdinal("content")),
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
}
