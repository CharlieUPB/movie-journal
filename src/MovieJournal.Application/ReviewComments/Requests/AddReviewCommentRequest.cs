using MovieJournal.Application.Common;

namespace MovieJournal.Application.ReviewComments.Requests;

public record AddReviewCommentRequest(
    Guid MovieReviewId,
    Guid UserId,
    string? Content) : IUserScopedRequest;
