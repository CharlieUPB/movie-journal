using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;

namespace MovieJournal.Application.UnitTests.MovieReviews.Queries;

public class ListMovieReviewsByUserIdQueryTests
{
    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenUserIdIsEmpty()
    {
        var repository = new FakeMovieReviewsRepository();
        var query = new ListMovieReviewsByUserIdQuery(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            query.Execute(new ListMovieReviewsByUserIdRequest(Guid.Empty)));

        Assert.Equal("User id is required", exception.Message);
        Assert.Equal(0, repository.ListAsyncCallCount);
    }

    [Fact]
    public async Task ShouldReturnOnlyReviewsProvidedByRepositoryForUser()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(userId: userId));
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(userId: Guid.NewGuid()));
        var query = new ListMovieReviewsByUserIdQuery(repository);

        var response = await query.Execute(new ListMovieReviewsByUserIdRequest(userId));

        Assert.Single(response.Reviews);
        Assert.Equal(userId, response.Reviews[0].UserId);
        Assert.Equal(userId, repository.LastCriteriaUsed?.UserId);
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenRepositoryReturnsNoReviewsForUser()
    {
        var repository = new FakeMovieReviewsRepository();
        var query = new ListMovieReviewsByUserIdQuery(repository);

        var response = await query.Execute(new ListMovieReviewsByUserIdRequest(Guid.NewGuid()));

        Assert.Empty(response.Reviews);
    }
}
