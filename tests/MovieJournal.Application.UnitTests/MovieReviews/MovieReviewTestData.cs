using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.UnitTests.MovieReviews;

internal static class MovieReviewTestData
{
    public const string ValidReviewContent = "A very enjoyable story with interesting characters and strong emotional moments.";
    public const string UpdatedReviewContent = "A very MUCH MUCH enjoyable story with interesting characters and strong emotional moments.";

    public static MovieReview CreateMovieReview(
        Guid? id = null,
        Guid? userId = null,
        string movieTitle = "Project Hail Mary",
        int? movieReleaseYear = 2026,
        string reviewTitle = "Great story",
        string reviewContent = ValidReviewContent,
        int reviewRating = 5,
        ReviewStatus status = ReviewStatus.Draft,
        bool? isDeleted = false)
    {
        return MovieReview.Rebuild(
            id ?? Guid.NewGuid(),
            userId ?? Guid.NewGuid(),
            movieTitle,
            movieReleaseYear,
            reviewTitle,
            reviewContent,
            reviewRating,
            status,
            DateTime.UtcNow,
            null,
            isDeleted);
    }
}
