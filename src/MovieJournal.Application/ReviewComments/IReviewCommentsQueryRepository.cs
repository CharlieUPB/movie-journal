using MovieJournal.Application.ReviewComments.Responses;

namespace MovieJournal.Application.ReviewComments;

public interface IReviewCommentsQueryRepository
{
    Task<IReadOnlyList<ReviewCommentResponse>> GetByMovieReviewIdAsync(
        Guid movieReviewId,
        Guid? currentUserId);
}
