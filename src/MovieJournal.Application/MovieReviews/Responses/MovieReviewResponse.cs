namespace MovieJournal.Application.MovieReviews.Responses;

public record MovieReviewResponse(
    Guid Id,
    Guid UserId,
    string MovieTitle,
    int? MovieReleaseYear,
    string ReviewTitle,
    string ReviewContent,
    int ReviewRating,
    string Status,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
