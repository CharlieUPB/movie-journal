using MovieJournal.Application.MovieReviews;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Enums;
using MovieJournal.Infrastructure.Persistence.Connection;
using System.Data;

namespace MovieJournal.Infrastructure.Persistence.MovieReviews;

public sealed class MovieReviewsRepository(ISqlConnectionFactory sqlConnectionFactory)
    : IMovieReviewsRepository
{
    public Task<MovieReview> CreateAsync(MovieReview entity)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
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
                updated_at,
                is_deleted
            )
            VALUES (
                @id,
                @user_id,
                @movie_title,
                @release_year,
                @review_title,
                @review_content,
                @rating,
                @status,
                @created_at,
                @updated_at,
                @is_deleted
            );
            """;

        AddParameter(command, "@id", entity.Id.ToString());
        AddParameter(command, "@user_id", entity.UserId.ToString());
        AddParameter(command, "@movie_title", entity.MovieInformation.MovieTitle);
        AddParameter(command, "@release_year", entity.MovieInformation.ReleaseYear);
        AddParameter(command, "@review_title", entity.ReviewInformation.ReviewTitle);
        AddParameter(command, "@review_content", entity.ReviewInformation.ReviewContent);
        AddParameter(command, "@rating", entity.ReviewInformation.Rating);
        AddParameter(command, "@status", (int)entity.Status);
        AddParameter(command, "@created_at", entity.CreatedAt.ToString("O"));
        AddParameter(command, "@updated_at", entity.UpdatedAt?.ToString("O"));
        AddParameter(command, "@is_deleted", entity.IsDeleted == true ? 1 : 0);

        command.ExecuteNonQuery();

        return Task.FromResult(entity);
    }

    public Task<MovieReview> UpdateAsync(MovieReview entity)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            UPDATE movie_reviews
            SET
                movie_title = @movie_title,
                release_year = @release_year,
                review_title = @review_title,
                review_content = @review_content,
                rating = @rating,
                status = @status,
                updated_at = @updated_at,
                is_deleted = @is_deleted
            WHERE id = @id;
            """;

        AddParameter(command, "@id", entity.Id.ToString());
        AddParameter(command, "@movie_title", entity.MovieInformation.MovieTitle);
        AddParameter(command, "@release_year", entity.MovieInformation.ReleaseYear);
        AddParameter(command, "@review_title", entity.ReviewInformation.ReviewTitle);
        AddParameter(command, "@review_content", entity.ReviewInformation.ReviewContent);
        AddParameter(command, "@rating", entity.ReviewInformation.Rating);
        AddParameter(command, "@status", (int)entity.Status);
        AddParameter(command, "@updated_at", entity.UpdatedAt?.ToString("O"));
        AddParameter(command, "@is_deleted", entity.IsDeleted == true ? 1 : 0);

        command.ExecuteNonQuery();

        return Task.FromResult(entity);
    }

    public Task<MovieReview> DeleteAsync(MovieReview entity)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            UPDATE movie_reviews
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

    public Task<MovieReview?> GetByIdAsync(Guid id)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                id,
                user_id,
                movie_title,
                release_year,
                review_title,
                review_content,
                rating,
                status,
                created_at,
                updated_at,
                is_deleted
            FROM movie_reviews
            WHERE id = @id
              AND COALESCE(is_deleted, 0) = 0
            LIMIT 1;
            """;

        AddParameter(command, "@id", id.ToString());

        using var reader = command.ExecuteReader();

        if (!reader.Read())
        {
            return Task.FromResult<MovieReview?>(null);
        }

        var review = MapToMovieReview(reader);

        return Task.FromResult<MovieReview?>(review);
    }

    public Task<IReadOnlyList<MovieReview>> ListAsync(MovieReviewQueryCriteria criteria)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        var conditions = new List<string>();

        if (!criteria.IncludeDeleted)
        {
            conditions.Add("COALESCE(is_deleted, 0) = 0");
        }

        if (criteria.UserId.HasValue)
        {
            conditions.Add("user_id = @user_id");
            AddParameter(command, "@user_id", criteria.UserId.Value.ToString());
        }

        if (criteria.Status.HasValue)
        {
            conditions.Add("status = @status");
            AddParameter(command, "@status", (int)criteria.Status.Value);
        }

        var whereClause = conditions.Count > 0
            ? $"WHERE {string.Join(" AND ", conditions)}"
            : string.Empty;

        command.CommandText = $"""
        SELECT
            id,
            user_id,
            movie_title,
            release_year,
            review_title,
            review_content,
            rating,
            status,
            created_at,
            updated_at,
            is_deleted
        FROM movie_reviews
        {whereClause}
        ORDER BY created_at DESC;
        """;

        using var reader = command.ExecuteReader();

        var reviews = new List<MovieReview>();

        while (reader.Read())
        {
            reviews.Add(MapToMovieReview(reader));
        }

        return Task.FromResult<IReadOnlyList<MovieReview>>(reviews);
    }

    private static void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }

    private static MovieReview MapToMovieReview(IDataRecord reader)
    {
        return MovieReview.Rebuild(
            Guid.Parse(reader.GetString(reader.GetOrdinal("id"))),
            Guid.Parse(reader.GetString(reader.GetOrdinal("user_id"))),
            reader.GetString(reader.GetOrdinal("movie_title")),
            reader.IsDBNull(reader.GetOrdinal("release_year"))
                ? null
                : reader.GetInt32(reader.GetOrdinal("release_year")),
            reader.GetString(reader.GetOrdinal("review_title")),
            reader.GetString(reader.GetOrdinal("review_content")),
            reader.GetInt32(reader.GetOrdinal("rating")),
            (ReviewStatus)reader.GetInt32(reader.GetOrdinal("status")),
            DateTime.Parse(reader.GetString(reader.GetOrdinal("created_at"))),
            reader.IsDBNull(reader.GetOrdinal("updated_at"))
                ? null
                : DateTime.Parse(reader.GetString(reader.GetOrdinal("updated_at"))),
            !reader.IsDBNull(reader.GetOrdinal("is_deleted")) &&
            reader.GetInt32(reader.GetOrdinal("is_deleted")) == 1);
    }

}