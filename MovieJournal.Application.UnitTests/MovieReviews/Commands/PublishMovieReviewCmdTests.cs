using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Enums;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Application.UnitTests.MovieReviews.Commands;

public class PublishMovieReviewCmdTests
{
    [Fact]
    public async Task ShouldPublishMovieReviewWhenUserIsOwner()
    {
        var userId = Guid.NewGuid();
        var review = CreateDraftReview(userId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new PublishMovieReviewCmd(repository);

        var response = await command.Execute(new PublishMovieReviewRequest(review.Id, userId));

        Assert.Equal("Published", response.Status);
        Assert.Equal(1, repository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenPublishingReviewThatDoesNotExist()
    {
        var repository = new FakeMovieReviewsRepository();
        var command = new PublishMovieReviewCmd(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            command.Execute(new PublishMovieReviewRequest(Guid.NewGuid(), Guid.NewGuid())));

        Assert.Equal("Movie review was not found", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenPublishingReviewOwnedByAnotherUser()
    {
        var review = CreateDraftReview(Guid.NewGuid());
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new PublishMovieReviewCmd(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            command.Execute(new PublishMovieReviewRequest(review.Id, Guid.NewGuid())));

        Assert.Equal("You are not allowed to publish this movie review", exception.Message);
        Assert.Equal(0, repository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldLetDomainExceptionBubbleUpWhenPublishingNonDraftReview()
    {
        var userId = Guid.NewGuid();
        var review = CreatePublishedReview(userId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new PublishMovieReviewCmd(repository);

        var exception = await Assert.ThrowsAsync<DomainException>(() =>
            command.Execute(new PublishMovieReviewRequest(review.Id, userId)));

        Assert.Equal("Only draft reviews can be published", exception.Message);
        Assert.Equal(0, repository.UpdateAsyncCallCount);
    }

    private static MovieReview CreateDraftReview(Guid userId)
    {
        return MovieReviewTestData.CreateMovieReview(userId: userId);
    }

    private static MovieReview CreatePublishedReview(Guid userId)
    {
        return MovieReviewTestData.CreateMovieReview(userId: userId, status: ReviewStatus.Published);
    }
}
