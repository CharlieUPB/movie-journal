using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.UnitTests.MovieReviews.Queries;

public class ListMovieReviewsQueryTests
{
    [Fact]
    public async Task ShouldReturnAllReviewsProvidedByRepository()
    {
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Draft));
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published));
        var query = new ListMovieReviewsQuery(repository);

        var response = await query.Execute(new ListMovieReviewsRequest());

        Assert.Equal(2, response.Reviews.Count);
        Assert.Equal(1, repository.ListAsyncCallCount);
        Assert.Null(repository.LastCriteriaUsed?.UserId);
        Assert.Null(repository.LastCriteriaUsed?.Status);
    }

    [Fact]
    public async Task ShouldReturnEmptyResponseListWhenRepositoryReturnsEmptyList()
    {
        var repository = new FakeMovieReviewsRepository();
        var query = new ListMovieReviewsQuery(repository);

        var response = await query.Execute(new ListMovieReviewsRequest());

        Assert.Empty(response.Reviews);
    }
}
