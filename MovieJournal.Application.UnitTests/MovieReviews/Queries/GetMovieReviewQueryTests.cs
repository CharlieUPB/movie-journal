using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.UnitTests.MovieReviews.Queries;

public class GetMovieReviewQueryTests
{
    [Fact]
    public async Task ShouldReturnPublishedReviewForAnonymousUser()
    {
        var review = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var query = new GetMovieReviewQuery(repository);

        var response = await query.Execute(new GetMovieReviewRequest(review.Id, null));

        Assert.Equal(review.Id, response.Id);
        Assert.Equal("Published", response.Status);
    }

    [Theory]
    [InlineData(ReviewStatus.Draft)]
    [InlineData(ReviewStatus.Archived)]
    public async Task ShouldReturnReviewWhenUserIsOwnerEvenIfNotPublished(ReviewStatus status)
    {
        var ownerId = Guid.NewGuid();
        var review = MovieReviewTestData.CreateMovieReview(userId: ownerId, status: status);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var query = new GetMovieReviewQuery(repository);

        var response = await query.Execute(new GetMovieReviewRequest(review.Id, ownerId));

        Assert.Equal(review.Id, response.Id);
        Assert.Equal(status.ToString(), response.Status);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenReviewDoesNotExist()
    {
        var repository = new FakeMovieReviewsRepository();
        var query = new GetMovieReviewQuery(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            query.Execute(new GetMovieReviewRequest(Guid.NewGuid(), Guid.NewGuid())));

        Assert.Equal("Movie review was not found", exception.Message);
    }

    [Theory]
    [InlineData(ReviewStatus.Draft)]
    [InlineData(ReviewStatus.Archived)]
    public async Task ShouldThrowUseCaseExceptionWhenCurrentUserIsNotOwnerAndReviewIsNotPublished(ReviewStatus status)
    {
        var review = MovieReviewTestData.CreateMovieReview(userId: Guid.NewGuid(), status: status);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var query = new GetMovieReviewQuery(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            query.Execute(new GetMovieReviewRequest(review.Id, Guid.NewGuid())));

        Assert.Equal("You are not allowed to view this movie review", exception.Message);
    }

    [Theory]
    [InlineData(ReviewStatus.Draft)]
    [InlineData(ReviewStatus.Archived)]
    public async Task ShouldRejectAnonymousUserWhenReviewIsNotPublished(ReviewStatus status)
    {
        var review = MovieReviewTestData.CreateMovieReview(status: status);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var query = new GetMovieReviewQuery(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            query.Execute(new GetMovieReviewRequest(review.Id, null)));

        Assert.Equal("You are not allowed to view this movie review", exception.Message);
    }
}
