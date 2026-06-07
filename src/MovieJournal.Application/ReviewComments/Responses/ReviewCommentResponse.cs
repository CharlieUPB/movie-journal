namespace MovieJournal.Application.ReviewComments.Responses;

public record ReviewCommentResponse(
    Guid Id,
    Guid MovieReviewId,
    Guid UserId,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
