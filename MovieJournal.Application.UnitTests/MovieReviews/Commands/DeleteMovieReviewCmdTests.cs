using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;

namespace MovieJournal.Application.UnitTests.MovieReviews.Commands;

public class DeleteMovieReviewCmdTests
{
    [Fact]
    public async Task ShouldNotThrowWhenReviewDoesNotExist()
    {
        var repository = new FakeMovieReviewsRepository();
        var command = new DeleteMovieReviewCmd(repository);
        var request = new DeleteMovieReviewRequest(Guid.NewGuid(), Guid.NewGuid());

        await command.Execute(request);

        Assert.Equal(0, repository.DeleteAsyncCallCount);
    }

    [Fact]
    public async Task ShouldNotAllowNonOwnerToDeleteReview()
    {
        var review = MovieReviewTestData.CreateMovieReview(userId: Guid.NewGuid());
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new DeleteMovieReviewCmd(repository);
        var request = new DeleteMovieReviewRequest(review.Id, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("You are not allowed to delete this movie review", exception.Message);
        Assert.Equal(0, repository.DeleteAsyncCallCount);
    }

    [Fact]
    public async Task ShouldDeleteReviewWhenUserIsOwner()
    {
        var ownerId = Guid.NewGuid();
        var review = MovieReviewTestData.CreateMovieReview(userId: ownerId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new DeleteMovieReviewCmd(repository);
        var request = new DeleteMovieReviewRequest(review.Id, ownerId);

        await command.Execute(request);

        Assert.Equal(1, repository.DeleteAsyncCallCount);
        Assert.Single(repository.DeletedEntities);
        Assert.True(review.IsDeleted);
    }
}
