namespace MovieJournal.Application.MovieReviews.Requests;

public record GetMovieReviewRequest(
    Guid MovieReviewId,
    Guid? UserId);
