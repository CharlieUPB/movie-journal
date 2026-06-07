using MovieJournal.Application.Common;

namespace MovieJournal.Application.MovieReviews.Requests;

public record ListMovieReviewsByUserIdRequest(Guid UserId): IUserScopedRequest;