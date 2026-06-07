using MovieJournal.Application.Common;

namespace MovieJournal.Application.MovieReviews.Requests;

public record UpdateMovieReviewRequest(
 Guid MovieReviewId,
 Guid UserId,
 string MovieTitle,
 string ReviewTitle,
 string ReviewContent,
 int ReviewRating,
 int? MovieReleaseYear) : IUserScopedRequest;
