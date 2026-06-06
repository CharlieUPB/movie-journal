using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.UnitTests.MovieReviews.Queries;

public class ListMovieReviewsByUserIdAndStatusQueryTests
{
    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenUserIdIsEmpty()
    {
        var repository = new FakeMovieReviewsRepository();
        var query = new ListMovieReviewsByUserIdAndStatusQuery(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            query.Execute(new ListMovieReviewsByUserIdAndStatusRequest(Guid.Empty, ReviewStatus.Published)));

        Assert.Equal("User id is required", exception.Message);
        Assert.Equal(0, repository.ListAsyncCallCount);
    }

    [Fact]
    public async Task ShouldReturnReviewsForGivenUserAndStatus()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(userId: userId, status: ReviewStatus.Published));
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(userId: userId, status: ReviewStatus.Draft));
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(userId: Guid.NewGuid(), status: ReviewStatus.Published));
        var query = new ListMovieReviewsByUserIdAndStatusQuery(repository);

        var response = await query.Execute(
            new ListMovieReviewsByUserIdAndStatusRequest(userId, ReviewStatus.Published));

        Assert.Single(response.Reviews);
        Assert.Equal(userId, response.Reviews[0].UserId);
        Assert.Equal("Published", response.Reviews[0].Status);
    }

    [Fact]
    public async Task ShouldPassExpectedUserIdAndStatusToRepository()
    {
        var userId = Guid.NewGuid();
        var repository = new FakeMovieReviewsRepository();
        var query = new ListMovieReviewsByUserIdAndStatusQuery(repository);

        await query.Execute(new ListMovieReviewsByUserIdAndStatusRequest(userId, ReviewStatus.Archived));

        Assert.Equal(userId, repository.LastCriteriaUsed?.UserId);
        Assert.Equal(ReviewStatus.Archived, repository.LastCriteriaUsed?.Status);
    }

    [Fact]
    public async Task ShouldReturnEmptyListWhenThereAreNoResults()
    {
        var repository = new FakeMovieReviewsRepository();
        var query = new ListMovieReviewsByUserIdAndStatusQuery(repository);

        var response = await query.Execute(
            new ListMovieReviewsByUserIdAndStatusRequest(Guid.NewGuid(), ReviewStatus.Published));

        Assert.Empty(response.Reviews);
    }
}
