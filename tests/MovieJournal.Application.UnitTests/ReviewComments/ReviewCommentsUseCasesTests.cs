using MovieJournal.Application.Exceptions;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.ReviewComments.Commands;
using MovieJournal.Application.ReviewComments.Queries;
using MovieJournal.Application.ReviewComments.Requests;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Application.UnitTests.MovieReviews;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Application.UnitTests.ReviewComments.Fakes;
using MovieJournal.Application.UnitTests.Users.Fakes;
using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Enums;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Application.UnitTests.ReviewComments;

public class ReviewCommentsUseCasesTests
{
    [Fact]
    public async Task ShouldAddCommentWhenReviewIsPublishedAndUserIsAuthenticated()
    {
        var userId = Guid.NewGuid();
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var userRepository = new FakeUserRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        userRepository.Users.Add(CreateUser(userId, "Carlos"));
        var command = new AddReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository, userRepository);
        var request = new AddReviewCommentRequest(movieReview.Id, userId, "Great review");

        var response = await command.Execute(request);

        Assert.Equal(1, reviewCommentsRepository.CreateAsyncCallCount);
        Assert.Equal(movieReview.Id, response.MovieReviewId);
        Assert.Equal("Carlos", response.OwnerName);
        Assert.True(response.IsOwner);
        Assert.Equal("Great review", response.Content);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenMovieReviewDoesNotExist()
    {
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var userRepository = new FakeUserRepository();
        var command = new AddReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository, userRepository);
        var request = new AddReviewCommentRequest(Guid.NewGuid(), Guid.NewGuid(), "Great review");

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("Movie review was not found", exception.Message);
        Assert.Equal(0, reviewCommentsRepository.CreateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldLetDomainExceptionBubbleUpWhenReviewIsDraft()
    {
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Draft);
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var userRepository = new FakeUserRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        var command = new AddReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository, userRepository);
        var request = new AddReviewCommentRequest(movieReview.Id, Guid.NewGuid(), "Great review");

        var exception = await Assert.ThrowsAsync<DomainException>(() => command.Execute(request));

        Assert.Equal("Comments are allowed only on published reviews", exception.Message);
        Assert.Equal(0, reviewCommentsRepository.CreateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldLetDomainExceptionBubbleUpWhenContentIsInvalid()
    {
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var userRepository = new FakeUserRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        var command = new AddReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository, userRepository);
        var request = new AddReviewCommentRequest(movieReview.Id, Guid.NewGuid(), "  ");

        var exception = await Assert.ThrowsAsync<DomainException>(() => command.Execute(request));

        Assert.Equal("Comment content is requiered and should not exceed 500 chars", exception.Message);
        Assert.Equal(0, reviewCommentsRepository.CreateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldReturnCommentsWhenReviewIsPublishedAndUserIsAnonymous()
    {
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var commentOwnerId = Guid.NewGuid();
        var reviewComment = CreateReviewComment(movieReview.Id, commentOwnerId, "First comment");
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsQueryRepository = new FakeReviewCommentsQueryRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        reviewCommentsQueryRepository.Comments.Add(CreateReviewCommentResponse(
            reviewComment.Id,
            movieReview.Id,
            "Maria",
            false,
            "First comment"));
        var query = new ListReviewCommentsQuery(movieReviewsRepository, reviewCommentsQueryRepository);
        var request = new ListReviewCommentsRequest(movieReview.Id, null);

        var response = await query.Execute(request);

        Assert.Single(response.Comments);
        Assert.Equal(reviewComment.Id, response.Comments[0].Id);
        Assert.Equal("Maria", response.Comments[0].OwnerName);
        Assert.False(response.Comments[0].IsOwner);
        Assert.Equal("First comment", response.Comments[0].Content);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenNonOwnerTriesToViewDraftReviewComments()
    {
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Draft);
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsQueryRepository = new FakeReviewCommentsQueryRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        var query = new ListReviewCommentsQuery(movieReviewsRepository, reviewCommentsQueryRepository);
        var request = new ListReviewCommentsRequest(movieReview.Id, Guid.NewGuid());

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => query.Execute(request));

        Assert.Equal("You are not allowed to view these review comments", exception.Message);
        Assert.Equal(0, reviewCommentsQueryRepository.GetByMovieReviewIdAsyncCallCount);
    }

    [Fact]
    public async Task ShouldUpdateCommentWhenUserIsAuthorAndReviewIsPublished()
    {
        var authorId = Guid.NewGuid();
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var reviewComment = CreateReviewComment(movieReview.Id, authorId, "Original comment");
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var userRepository = new FakeUserRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        reviewCommentsRepository.Comments.Add(reviewComment);
        userRepository.Users.Add(CreateUser(authorId, "Carlos"));
        var command = new UpdateReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository, userRepository);
        var request = new UpdateReviewCommentRequest(reviewComment.Id, authorId, "Updated comment");

        var response = await command.Execute(request);

        Assert.Equal(1, reviewCommentsRepository.UpdateAsyncCallCount);
        Assert.Equal("Carlos", response.OwnerName);
        Assert.True(response.IsOwner);
        Assert.Equal("Updated comment", response.Content);
        Assert.Equal("Updated comment", reviewComment.Content);
    }

    [Fact]
    public async Task ShouldThrowUseCaseExceptionWhenUserIsNotCommentAuthor()
    {
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var reviewComment = CreateReviewComment(movieReview.Id, Guid.NewGuid(), "Original comment");
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var userRepository = new FakeUserRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        reviewCommentsRepository.Comments.Add(reviewComment);
        var command = new UpdateReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository, userRepository);
        var request = new UpdateReviewCommentRequest(reviewComment.Id, Guid.NewGuid(), "Updated comment");

        var exception = await Assert.ThrowsAsync<UseCaseException>(() => command.Execute(request));

        Assert.Equal("You are not allowed to update this review comment", exception.Message);
        Assert.Equal(0, reviewCommentsRepository.UpdateAsyncCallCount);
    }

    [Fact]
    public async Task ShouldDeleteCommentWhenUserIsAuthorAndReviewIsPublished()
    {
        var authorId = Guid.NewGuid();
        var movieReview = MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published);
        var reviewComment = CreateReviewComment(movieReview.Id, authorId, "Original comment");
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        movieReviewsRepository.Reviews.Add(movieReview);
        reviewCommentsRepository.Comments.Add(reviewComment);
        var command = new DeleteReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository);
        var request = new DeleteReviewCommentRequest(reviewComment.Id, authorId);

        await command.Execute(request);

        Assert.Equal(1, reviewCommentsRepository.DeleteAsyncCallCount);
        Assert.True(reviewComment.IsDeleted);
    }

    [Fact]
    public async Task ShouldBeIdempotentWhenCommentDoesNotExist()
    {
        var movieReviewsRepository = new FakeMovieReviewsRepository();
        var reviewCommentsRepository = new FakeReviewCommentsRepository();
        var command = new DeleteReviewCommentCmd(movieReviewsRepository, reviewCommentsRepository);
        var request = new DeleteReviewCommentRequest(Guid.NewGuid(), Guid.NewGuid());

        await command.Execute(request);

        Assert.Equal(0, reviewCommentsRepository.DeleteAsyncCallCount);
        Assert.Equal(0, movieReviewsRepository.GetByIdAsyncCallCount);
    }

    private static ReviewComment CreateReviewComment(
        Guid movieReviewId,
        Guid userId,
        string content,
        bool? isDeleted = false)
    {
        return ReviewComment.Rebuild(
            Guid.NewGuid(),
            movieReviewId,
            userId,
            content,
            DateTime.UtcNow,
            null,
            isDeleted);
    }

    private static User CreateUser(Guid id, string displayName)
    {
        return User.Rebuild(
            id,
            displayName,
            $"{displayName.ToLowerInvariant()}@example.com",
            "hashed-password",
            DateTime.UtcNow,
            null,
            false);
    }

    private static ReviewCommentResponse CreateReviewCommentResponse(
        Guid id,
        Guid movieReviewId,
        string ownerName,
        bool isOwner,
        string content)
    {
        return new ReviewCommentResponse(
            id,
            movieReviewId,
            ownerName,
            isOwner,
            content,
            DateTime.UtcNow,
            null);
    }
}
