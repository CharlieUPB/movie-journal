using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.ReviewComments.Mappers;

public static class ReviewCommentMapper
{
    public static ReviewCommentResponse ToResponse(
        ReviewComment reviewComment,
        string ownerName,
        bool isOwner)
    {
        return new ReviewCommentResponse(
            reviewComment.Id,
            reviewComment.MovieReviewId,
            ownerName,
            isOwner,
            reviewComment.Content,
            reviewComment.CreatedAt,
            reviewComment.UpdatedAt);
    }

}
