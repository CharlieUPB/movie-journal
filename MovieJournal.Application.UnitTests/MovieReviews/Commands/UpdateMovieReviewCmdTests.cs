using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Enums;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Application.UnitTests.MovieReviews.Commands;

public class UpdateMovieReviewCmdTests
{
    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenReviewDoesNotExist()
    {
        var repository = new FakeMovieReviewsRepository();
        var command = new UpdateMovieReviewCmd(repository);
        var request = CreateRequest(Guid.NewGuid(), Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("Movie review was not found", exception.Message);
        Assert.Equal(0, repository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldNotAllowNonOwnerToUpdateReview()
    {
        var ownerId = Guid.NewGuid();
        var review = MovieReviewTestData.CreateMovieReview(userId: ownerId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new UpdateMovieReviewCmd(repository);
        var request = CreateRequest(review.Id, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("You are not allowed to update this movie review", exception.Message);
        Assert.Equal(0, repository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldUpdateReviewWhenUserIsOwner()
    {
        var ownerId = Guid.NewGuid();
        var review = MovieReviewTestData.CreateMovieReview(userId: ownerId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new UpdateMovieReviewCmd(repository);
        var request = CreateRequest(review.Id, ownerId);

        var response = await command.Execute(request);

        Assert.Equal(1, repository.UpdateAsyncCallCount);
        Assert.Single(repository.UpdatedEntities);
        Assert.Equal("Updated movie", response.MovieTitle);
        Assert.Equal("Updated review", response.ReviewTitle);
        Assert.Equal(4, response.ReviewRating);
        Assert.Equal(2024, response.MovieReleaseYear);
    }

    [Fact]
    public async Task ShouldLetDomainExceptionBubbleUpWhenDomainRejectsUpdate()
    {
        var ownerId = Guid.NewGuid();
        var review = MovieReviewTestData.CreateMovieReview(
            userId: ownerId,
            status: ReviewStatus.Archived);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new UpdateMovieReviewCmd(repository);
        var request = CreateRequest(review.Id, ownerId);

        var exception = await Assert.ThrowsAsync<DomainException>(() => command.Execute(request));

        Assert.Equal("Archived reviews cannot be updated", exception.Message);
        Assert.Equal(0, repository.UpdateAsyncCallCount);
    }

    private static UpdateMovieReviewRequest CreateRequest(Guid movieReviewId, Guid userId)
    {
        return new UpdateMovieReviewRequest(
            movieReviewId,
            userId,
            "Updated movie",
            "Updated review",
            MovieReviewTestData.UpdatedReviewContent,
            4,
            2024);
    }
}
