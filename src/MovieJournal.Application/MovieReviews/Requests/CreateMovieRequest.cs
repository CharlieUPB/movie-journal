using MovieJournal.Application.Common;

namespace MovieJournal.Application.MovieReviews.Inputs;

public record CreateMovieRequest(
    Guid UserId,
    string MovieTitle,
    string ReviewTitle,
    string ReviewContent,
    int ReviewRating,
    int? MovieReleaseYear
    ) : IUserScopedRequest;

