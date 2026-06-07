using MovieJournal.Application.ReviewComments;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.UnitTests.ReviewComments.Fakes;

internal class FakeReviewCommentsRepository : IReviewCommentsRepository
{
    public List<ReviewComment> Comments { get; } = new();
    public List<ReviewComment> CreatedEntities { get; } = new();
    public List<ReviewComment> UpdatedEntities { get; } = new();
    public List<ReviewComment> DeletedEntities { get; } = new();

    public int CreateAsyncCallCount { get; private set; }
    public int UpdateAsyncCallCount { get; private set; }
    public int DeleteAsyncCallCount { get; private set; }
    public int GetByIdAsyncCallCount { get; private set; }
    public int GetByMovieReviewIdAsyncCallCount { get; private set; }

    public Task<ReviewComment> CreateAsync(ReviewComment reviewComment)
    {
        CreateAsyncCallCount++;
        CreatedEntities.Add(reviewComment);
        Comments.Add(reviewComment);

        return Task.FromResult(reviewComment);
    }

    public Task<ReviewComment?> GetByIdAsync(Guid id)
    {
        GetByIdAsyncCallCount++;

        return Task.FromResult(Comments.FirstOrDefault(comment => comment.Id == id));
    }

    public Task<IReadOnlyList<ReviewComment>> GetByMovieReviewIdAsync(Guid movieReviewId)
    {
        GetByMovieReviewIdAsyncCallCount++;

        return Task.FromResult<IReadOnlyList<ReviewComment>>(
            Comments
                .Where(comment => comment.MovieReviewId == movieReviewId)
                .ToList());
    }

    public Task<ReviewComment> UpdateAsync(ReviewComment reviewComment)
    {
        UpdateAsyncCallCount++;
        UpdatedEntities.Add(reviewComment);

        return Task.FromResult(reviewComment);
    }

    public Task<ReviewComment> DeleteAsync(ReviewComment reviewComment)
    {
        DeleteAsyncCallCount++;
        DeletedEntities.Add(reviewComment);

        return Task.FromResult(reviewComment);
    }
}
