using MovieJournal.Application.Common;

namespace MovieJournal.Application.MovieReviews.Requests;

public record DeleteMovieReviewRequest(
Guid MovieReviewId,
Guid UserId) : IUserScopedRequest;
