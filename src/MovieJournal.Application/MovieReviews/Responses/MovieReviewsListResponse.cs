namespace MovieJournal.Application.MovieReviews.Responses;

public record MovieReviewsListResponse(IReadOnlyList<MovieReviewResponse> Reviews);