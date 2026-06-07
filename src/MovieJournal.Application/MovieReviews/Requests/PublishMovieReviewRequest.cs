using MovieJournal.Application.Common;

namespace MovieJournal.Application.MovieReviews.Requests;

public record PublishMovieReviewRequest(
    Guid MovieReviewId,
    Guid UserId) : IUserScopedRequest;
