using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.UnitTests.MovieReviews.Commands;

public class ArchiveMovieReviewCmdTests
{
    [Fact]
    public async Task ShouldArchiveMovieReviewWhenUserIsOwner()
    {
        var userId = Guid.NewGuid();
        var review = CreatePublishedReview(userId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new ArchiveMovieReviewCmd(repository);

        var response = await command.Execute(new ArchiveMovieReviewRequest(review.Id, userId));

        Assert.Equal("Archived", response.Status);
        Assert.Equal(1, repository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenArchivingReviewThatDoesNotExist()
    {
        var repository = new FakeMovieReviewsRepository();
        var command = new ArchiveMovieReviewCmd(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            command.Execute(new ArchiveMovieReviewRequest(Guid.NewGuid(), Guid.NewGuid())));

        Assert.Equal("Movie review was not found", exception.Message);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenArchivingReviewOwnedByAnotherUser()
    {
        var review = CreateDraftReview(Guid.NewGuid());
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new ArchiveMovieReviewCmd(repository);

        var exception = await Assert.ThrowsAsync<UseCaseException>(() =>
            command.Execute(new ArchiveMovieReviewRequest(review.Id, Guid.NewGuid())));

        Assert.Equal("You are not allowed to archive this movie review", exception.Message);
        Assert.Equal(0, repository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldBeIdempotentWhenArchivingAlreadyArchivedReview()
    {
        var userId = Guid.NewGuid();
        var review = CreateArchivedReview(userId);
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(review);
        var command = new ArchiveMovieReviewCmd(repository);

        var response = await command.Execute(new ArchiveMovieReviewRequest(review.Id, userId));

        Assert.Equal("Archived", response.Status);
        Assert.Equal(1, repository.UpdateAsyncCallCount);
    }

    private static MovieReview CreateDraftReview(Guid userId)
    {
        return MovieReviewTestData.CreateMovieReview(userId: userId);
    }

    private static MovieReview CreatePublishedReview(Guid userId)
    {
        return MovieReviewTestData.CreateMovieReview(userId: userId, status: ReviewStatus.Published);
    }

    private static MovieReview CreateArchivedReview(Guid userId)
    {
        return MovieReviewTestData.CreateMovieReview(userId: userId, status: ReviewStatus.Archived);
    }
}
