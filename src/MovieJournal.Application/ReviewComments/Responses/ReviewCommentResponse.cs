namespace MovieJournal.Application.ReviewComments.Responses;

public record ReviewCommentResponse(
    Guid Id,
    Guid MovieReviewId,
    string OwnerName,
    bool IsOwner,
    string Content,
    DateTime CreatedAt,
    DateTime? UpdatedAt);
