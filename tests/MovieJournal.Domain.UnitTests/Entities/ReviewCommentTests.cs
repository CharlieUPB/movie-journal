using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Exceptions;

namespace MovieJournal.Domain.UnitTests.Entities;

public class ReviewCommentTests
{
    [Fact]
    public void ShouldSaveReviewComment()
    {
        var movieReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var comment = "  I could not agree more, this has become my favorite movie  ";

        var reviewComment = ReviewComment.Create(movieReviewId, userId, comment);

        Assert.Equal(movieReviewId, reviewComment.MovieReviewId);
        Assert.Equal(userId, reviewComment.UserId);
        Assert.Equal(comment.Trim(), reviewComment.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    [InlineData("I completely agree with your review, especially the part about the emotional impact of the story and how the characters feel deeply human despite the science fiction setting. What I liked the most is that the movie does not rely only on visual effects or action scenes, but also builds tension through personal decisions, sacrifice, friendship, and hope. The pacing feels intentional, the performances are convincing, and the ending leaves you thinking about the story long after the movie is over. I would definitely recommend this movie to anyone who enjoys thoughtful sci-fi with strong emotional moments.")]
    public void Create_WhenCommentIsInvalid_ShouldThrowDomainException(string? comment)
    {
        var exception = Assert.Throws<DomainException>(() =>
            ReviewComment.Create(Guid.NewGuid(), Guid.NewGuid(), comment));

        Assert.Equal("Comment content is requiered and should not exceed 500 chars", exception.Message);
    }

    [Fact]
    public void ShouldRebuildReviewComment()
    {
        var id = Guid.NewGuid();
        var movieReviewId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var createdAt = DateTime.UtcNow.AddDays(-2);
        var updatedAt = DateTime.UtcNow.AddDays(-1);

        var reviewComment = ReviewComment.Rebuild(
            id,
            movieReviewId,
            userId,
            "Stored comment",
            createdAt,
            updatedAt,
            true);

        Assert.Equal(id, reviewComment.Id);
        Assert.Equal(movieReviewId, reviewComment.MovieReviewId);
        Assert.Equal(userId, reviewComment.UserId);
        Assert.Equal("Stored comment", reviewComment.Content);
        Assert.Equal(createdAt, reviewComment.CreatedAt);
        Assert.Equal(updatedAt, reviewComment.UpdatedAt);
        Assert.True(reviewComment.IsDeleted);
    }

    [Fact]
    public void UpdateComment_WhenContentIsValid_ShouldUpdateContent()
    {
        var reviewComment = ReviewComment.Create(Guid.NewGuid(), Guid.NewGuid(), "Original comment");

        reviewComment.UpdateComment("  Updated comment  ");

        Assert.Equal("Updated comment", reviewComment.Content);
        Assert.NotNull(reviewComment.UpdatedAt);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    [InlineData("I completely agree with your review, especially the part about the emotional impact of the story and how the characters feel deeply human despite the science fiction setting. What I liked the most is that the movie does not rely only on visual effects or action scenes, but also builds tension through personal decisions, sacrifice, friendship, and hope. The pacing feels intentional, the performances are convincing, and the ending leaves you thinking about the story long after the movie is over. I would definitely recommend this movie to anyone who enjoys thoughtful sci-fi with strong emotional moments.")]
    public void UpdateComment_WhenContentIsInvalid_ShouldThrowDomainException(string? comment)
    {
        var reviewComment = ReviewComment.Create(Guid.NewGuid(), Guid.NewGuid(), "Original comment");

        var exception = Assert.Throws<DomainException>(() =>
            reviewComment.UpdateComment(comment));

        Assert.Equal("Comment content is requiered and should not exceed 500 chars", exception.Message);
    }

    [Fact]
    public void Delete_WhenCommentIsNotDeleted_ShouldMarkCommentAsDeleted()
    {
        var reviewComment = ReviewComment.Create(Guid.NewGuid(), Guid.NewGuid(), "Original comment");

        reviewComment.Delete();

        Assert.True(reviewComment.IsDeleted);
        Assert.NotNull(reviewComment.UpdatedAt);
    }

    [Fact]
    public void Delete_WhenCommentIsAlreadyDeleted_ShouldBeIdempotent()
    {
        var reviewComment = ReviewComment.Create(Guid.NewGuid(), Guid.NewGuid(), "Original comment");
        reviewComment.Delete();
        var updatedAt = reviewComment.UpdatedAt;

        reviewComment.Delete();

        Assert.True(reviewComment.IsDeleted);
        Assert.Equal(updatedAt, reviewComment.UpdatedAt);
    }
}
