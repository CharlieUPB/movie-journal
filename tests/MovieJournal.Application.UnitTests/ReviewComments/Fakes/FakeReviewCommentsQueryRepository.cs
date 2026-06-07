using MovieJournal.Application.ReviewComments;
using MovieJournal.Application.ReviewComments.Responses;

namespace MovieJournal.Application.UnitTests.ReviewComments.Fakes;

internal class FakeReviewCommentsQueryRepository : IReviewCommentsQueryRepository
{
    public List<ReviewCommentResponse> Comments { get; } = new();
    public int GetByMovieReviewIdAsyncCallCount { get; private set; }

    public Task<IReadOnlyList<ReviewCommentResponse>> GetByMovieReviewIdAsync(
        Guid movieReviewId,
        Guid? currentUserId)
    {
        GetByMovieReviewIdAsyncCallCount++;

        return Task.FromResult<IReadOnlyList<ReviewCommentResponse>>(
            Comments
                .Where(comment => comment.MovieReviewId == movieReviewId)
                .ToList());
    }
}
