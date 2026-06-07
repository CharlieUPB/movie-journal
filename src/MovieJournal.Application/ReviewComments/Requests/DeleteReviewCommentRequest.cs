using MovieJournal.Application.Common;

namespace MovieJournal.Application.ReviewComments.Requests;

public record DeleteReviewCommentRequest(
    Guid CommentId,
    Guid UserId) : IUserScopedRequest;
