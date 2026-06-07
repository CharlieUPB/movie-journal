namespace MovieJournal.Infrastructure.IntegrationTests.Persistence;

public class ReviewCommentsRepositoryTests
{
    [Fact]
    public async Task CreateAsync_ShouldInsertReviewComment()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);
        var reviewComment = TestDatabase.CreateValidReviewComment(
            movieReview.Id,
            content: "This comment should be saved.");

        await database.ReviewCommentsRepository.CreateAsync(reviewComment);

        var savedComment = await database.ReviewCommentsRepository.GetByIdAsync(reviewComment.Id);

        Assert.NotNull(savedComment);
        Assert.Equal(reviewComment.Id, savedComment.Id);
        Assert.Equal(movieReview.Id, savedComment.MovieReviewId);
        Assert.Equal(TestDatabase.DemoUserId, savedComment.UserId);
        Assert.Equal("This comment should be saved.", savedComment.Content);
        Assert.Equal(reviewComment.CreatedAt, savedComment.CreatedAt);
    }

    [Fact]
    public async Task GetByIdAsync_WhenCommentDoesNotExist_ShouldReturnNull()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);

        var savedComment = await database.ReviewCommentsRepository.GetByIdAsync(Guid.NewGuid());

        Assert.Null(savedComment);
    }

    [Fact]
    public async Task GetByMovieReviewIdAsync_ShouldReturnOnlyCommentsForThatMovieReview()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var firstReview = TestDatabase.CreateValidMovieReview(movieTitle: "First Movie");
        var secondReview = TestDatabase.CreateValidMovieReview(movieTitle: "Second Movie");
        await database.Repository.CreateAsync(firstReview);
        await database.Repository.CreateAsync(secondReview);
        var firstComment = TestDatabase.CreateValidReviewComment(firstReview.Id, content: "First review comment");
        var secondComment = TestDatabase.CreateValidReviewComment(secondReview.Id, content: "Second review comment");
        await database.ReviewCommentsRepository.CreateAsync(firstComment);
        await database.ReviewCommentsRepository.CreateAsync(secondComment);

        var comments = await database.ReviewCommentsRepository.GetByMovieReviewIdAsync(firstReview.Id);

        Assert.NotEmpty(comments);
        Assert.All(comments, comment => Assert.Equal(firstReview.Id, comment.MovieReviewId));
        Assert.Contains(comments, comment => comment.Id == firstComment.Id);
        Assert.DoesNotContain(comments, comment => comment.Id == secondComment.Id);
    }

    [Fact]
    public async Task UpdateAsync_ShouldPersistUpdatedContent()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);
        var reviewComment = TestDatabase.CreateValidReviewComment(movieReview.Id, content: "Original comment");
        await database.ReviewCommentsRepository.CreateAsync(reviewComment);

        reviewComment.UpdateComment("Updated comment");
        await database.ReviewCommentsRepository.UpdateAsync(reviewComment);

        var savedComment = await database.ReviewCommentsRepository.GetByIdAsync(reviewComment.Id);

        Assert.NotNull(savedComment);
        Assert.Equal("Updated comment", savedComment.Content);
        Assert.NotNull(savedComment.UpdatedAt);
    }

    [Fact]
    public async Task DeleteAsync_ShouldSoftDeleteComment()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);
        var reviewComment = TestDatabase.CreateValidReviewComment(movieReview.Id);
        await database.ReviewCommentsRepository.CreateAsync(reviewComment);

        reviewComment.Delete();
        await database.ReviewCommentsRepository.DeleteAsync(reviewComment);

        var savedComment = await database.ReviewCommentsRepository.GetByIdAsync(reviewComment.Id);

        Assert.Null(savedComment);
    }

    [Fact]
    public async Task GetByMovieReviewIdAsync_ShouldExcludeDeletedComments()
    {
        using var database = await TestDatabase.CreateAsync(clearMovieReviewData: true);
        var movieReview = TestDatabase.CreateValidMovieReview();
        await database.Repository.CreateAsync(movieReview);
        var activeComment = TestDatabase.CreateValidReviewComment(movieReview.Id, content: "Active comment");
        var deletedComment = TestDatabase.CreateValidReviewComment(movieReview.Id, content: "Deleted comment");
        await database.ReviewCommentsRepository.CreateAsync(activeComment);
        await database.ReviewCommentsRepository.CreateAsync(deletedComment);
        deletedComment.Delete();
        await database.ReviewCommentsRepository.DeleteAsync(deletedComment);

        var comments = await database.ReviewCommentsRepository.GetByMovieReviewIdAsync(movieReview.Id);

        var comment = Assert.Single(comments);
        Assert.Equal(activeComment.Id, comment.Id);
        Assert.DoesNotContain(comments, reviewComment => reviewComment.Id == deletedComment.Id);
    }
}
