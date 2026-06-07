using MovieJournal.Application.Common;

namespace MovieJournal.Application.MovieReviews.Requests;

public record ArchiveMovieReviewRequest(
    Guid MovieReviewId,
    Guid UserId) : IUserScopedRequest;
