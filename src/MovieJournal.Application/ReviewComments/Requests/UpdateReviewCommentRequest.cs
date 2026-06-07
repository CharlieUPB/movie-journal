using MovieJournal.Application.Common;

namespace MovieJournal.Application.ReviewComments.Requests;

public record UpdateReviewCommentRequest(
    Guid CommentId,
    Guid UserId,
    string? Content) : IUserScopedRequest;
