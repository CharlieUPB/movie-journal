namespace MovieJournal.Application.ReviewComments.Responses;

public record ReviewCommentsListResponse(IReadOnlyList<ReviewCommentResponse> Comments);
