namespace MovieJournal.Application.ReviewComments.Requests;

public record ListReviewCommentsRequest(
    Guid MovieReviewId,
    Guid? UserId);
