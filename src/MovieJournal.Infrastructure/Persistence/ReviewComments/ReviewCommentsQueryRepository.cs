using MovieJournal.Application.ReviewComments;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Infrastructure.Persistence.Connection;
using System.Data;
using System.Globalization;

namespace MovieJournal.Infrastructure.Persistence.ReviewComments;

public class ReviewCommentsQueryRepository(ISqlConnectionFactory sqlConnectionFactory)
    : IReviewCommentsQueryRepository
{
    public Task<IReadOnlyList<ReviewCommentResponse>> GetByMovieReviewIdAsync(
        Guid movieReviewId,
        Guid? currentUserId)
    {
        using var connection = sqlConnectionFactory.GetOpenConnection();
        using var command = connection.CreateCommand();

        command.CommandText = """
            SELECT
                rc.id,
                rc.movie_review_id,
                u.display_name AS owner_name,
                CASE
                    WHEN @current_user_id IS NOT NULL
                     AND rc.user_id = @current_user_id
                    THEN 1
                    ELSE 0
                END AS is_owner,
                rc.content,
                rc.created_at,
                rc.updated_at
            FROM review_comments rc
            INNER JOIN users u
                ON u.id = rc.user_id
               AND COALESCE(u.is_deleted, 0) = 0
            WHERE rc.movie_review_id = @movie_review_id
              AND COALESCE(rc.is_deleted, 0) = 0
            ORDER BY rc.created_at ASC;
            """;

        AddParameter(command, "@movie_review_id", movieReviewId.ToString());
        AddParameter(command, "@current_user_id", currentUserId?.ToString());

        using var reader = command.ExecuteReader();

        var reviewComments = new List<ReviewCommentResponse>();

        while (reader.Read())
        {
            reviewComments.Add(MapToResponse(reader));
        }

        return Task.FromResult<IReadOnlyList<ReviewCommentResponse>>(reviewComments);
    }

    private static void AddParameter(IDbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;

        command.Parameters.Add(parameter);
    }

    private static ReviewCommentResponse MapToResponse(IDataRecord reader)
    {
        return new ReviewCommentResponse(
            Guid.Parse(reader.GetString(reader.GetOrdinal("id"))),
            Guid.Parse(reader.GetString(reader.GetOrdinal("movie_review_id"))),
            reader.GetString(reader.GetOrdinal("owner_name")),
            reader.GetInt32(reader.GetOrdinal("is_owner")) == 1,
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
                    DateTimeStyles.RoundtripKind));
    }
}
