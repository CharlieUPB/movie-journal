using MovieJournal.Application.Common;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.MovieReviews.Requests;

public sealed record ListMovieReviewsByUserIdAndStatusRequest(Guid UserId, ReviewStatus Status): IUserScopedRequest;