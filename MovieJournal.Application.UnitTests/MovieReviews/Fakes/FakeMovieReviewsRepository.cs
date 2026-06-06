using MovieJournal.Application.MovieReviews;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.UnitTests.MovieReviews.Fakes;

internal class FakeMovieReviewsRepository : IMovieReviewsRepository
{
    public List<MovieReview> Reviews { get; } = new();
    public List<MovieReview> CreatedEntities { get; } = new();
    public List<MovieReview> UpdatedEntities { get; } = new();
    public List<MovieReview> DeletedEntities { get; } = new();

    public MovieReviewQueryCriteria? LastCriteriaUsed { get; private set; }
    public int CreateAsyncCallCount { get; private set; }
    public int UpdateAsyncCallCount { get; private set; }
    public int DeleteAsyncCallCount { get; private set; }
    public int GetByIdAsyncCallCount { get; private set; }
    public int ListAsyncCallCount { get; private set; }

    public Task<MovieReview> CreateAsync(MovieReview movieReview)
    {
        CreateAsyncCallCount++;
        CreatedEntities.Add(movieReview);
        Reviews.Add(movieReview);

        return Task.FromResult(movieReview);
    }

    public Task<MovieReview> UpdateAsync(MovieReview movieReview)
    {
        UpdateAsyncCallCount++;
        UpdatedEntities.Add(movieReview);

        return Task.FromResult(movieReview);
    }

    public Task<MovieReview> DeleteAsync(MovieReview movieReview)
    {
        DeleteAsyncCallCount++;
        DeletedEntities.Add(movieReview);

        return Task.FromResult(movieReview);
    }

    public Task<MovieReview?> GetByIdAsync(Guid id)
    {
        GetByIdAsyncCallCount++;

        return Task.FromResult(Reviews.FirstOrDefault(review => review.Id == id));
    }

    public Task<IReadOnlyList<MovieReview>> ListAsync(MovieReviewQueryCriteria criteria)
    {
        ListAsyncCallCount++;
        LastCriteriaUsed = criteria;

        IEnumerable<MovieReview> query = Reviews;

        if (!criteria.IncludeDeleted)
        {
            query = query.Where(review => review.IsDeleted != true);
        }

        if (criteria.UserId.HasValue)
        {
            query = query.Where(review => review.UserId == criteria.UserId.Value);
        }

        if (criteria.Status.HasValue)
        {
            query = query.Where(review => review.Status == criteria.Status.Value);
        }

        return Task.FromResult<IReadOnlyList<MovieReview>>(query.ToList());
    }
}
