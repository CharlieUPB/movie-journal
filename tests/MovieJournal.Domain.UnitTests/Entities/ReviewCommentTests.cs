using MovieJournal.Domain.Entities;
using MovieJournal.Domain.Exceptions;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Domain.UnitTests.Entities;

public class ReviewCommentTests
{
    [Fact]
    public void ShouldSaveReviewComment()
    {
        var movieReview = CreateMovieReview();

        var comment = "I could not agree more, this has become my favorite movie";

        var reviewComment = ReviewComment.Create(movieReview.Id, Guid.NewGuid(), comment);

        Assert.Equal(comment, reviewComment.Content);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("  ")]
    [InlineData("I completely agree with your review, especially the part about the emotional impact of the story and how the characters feel deeply human despite the science fiction setting. What I liked the most is that the movie does not rely only on visual effects or action scenes, but also builds tension through personal decisions, sacrifice, friendship, and hope. The pacing feels intentional, the performances are convincing, and the ending leaves you thinking about the story long after the movie is over. I would definitely recommend this movie to anyone who enjoys thoughtful sci-fi with strong emotional moments.")]
    public void ShouldThrowDomainExceptionWhenCommentIsInvalid(string? comment)
    {
        var movieReview = CreateMovieReview();

        var exception = Assert.Throws<DomainException>(() => ReviewComment.Create(movieReview.Id, Guid.NewGuid(), comment));

        Assert.Equal("Comment content is requiered and should not exceed 500 chars", exception.Message);
    }


    private static MovieReview CreateMovieReview()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var movieInformation = MovieInformation.Create("Dune", today, 2021);
        var reviewInformation = ReviewInformation.Create(
            "One of the greatest sci-fi movie",
            "Denis Villeneuve has crafted a breathtaking sci-fi masterpiece. The cinematography and sweeping desert visuals are stunning, while the incredible cast—especially Timothée Chalamet and Zendaya—delivers deeply emotional performances. An absolute must-watch on the biggest screen possible!",
            5);

        return MovieReview.Create(Guid.NewGuid(), movieInformation, reviewInformation);
    }
}
