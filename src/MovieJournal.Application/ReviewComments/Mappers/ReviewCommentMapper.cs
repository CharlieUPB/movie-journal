using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.ReviewComments.Mappers;

public static class ReviewCommentMapper
{
    public static ReviewCommentResponse ToResponse(ReviewComment reviewComment)
    {
        return new ReviewCommentResponse(
            reviewComment.Id,
            reviewComment.MovieReviewId,
            reviewComment.UserId,
            reviewComment.Content,
            reviewComment.CreatedAt,
            reviewComment.UpdatedAt);
    }

    public static IReadOnlyList<ReviewCommentResponse> ToResponseList(IReadOnlyList<ReviewComment> reviewComments)
    {
        return reviewComments
            .Select(ToResponse)
            .ToList();
    }
}
