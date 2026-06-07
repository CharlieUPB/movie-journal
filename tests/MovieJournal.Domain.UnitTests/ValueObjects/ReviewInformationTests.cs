using MovieJournal.Domain.Exceptions;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Domain.UnitTests.ValueObjects;

public class ReviewInformationTests
{

    [Fact]
    public void ShouldSaveReviewInformation()
    {
        var title = "One of the greatest sci-fi movie";
        var content = "Denis Villeneuve has crafted a breathtaking sci-fi masterpiece. The cinematography and sweeping desert visuals are stunning, while the incredible cast—especially Timothée Chalamet and Zendaya—delivers deeply emotional performances. An absolute must-watch on the biggest screen possible!";
        int rating = 5;

        var reviewInformation = ReviewInformation.Create(title, content, rating);

        Assert.Equal(title, reviewInformation.ReviewTitle);
        Assert.Equal(content, reviewInformation.ReviewContent);
        Assert.Equal(rating, reviewInformation.Rating);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void ShouldThrowDomainExceptionWhenTitleIsInvalid(string? title)
    {
        var exception = Assert.Throws<DomainException>(() => CreateReviewInformation(title));

        Assert.Equal("Review title is required", exception.Message);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("A masterpiece of modern cinema.")]
    public void ShouldThrowDomainExceptionWhenReviewContentIsInvalid(string? content)
    {
        var exception = Assert.Throws<DomainException>(() => CreateReviewInformation(content: content));

        Assert.Equal("Content is required and must be minimum of 50 characters", exception.Message);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(10)]
    [InlineData(null)]
    public void ShouldThrowDomainExceptionWhenRatingIsInvalid(int? rating)
    {
        var exception = Assert.Throws<DomainException>(() => CreateReviewInformation(rating: rating));

        Assert.Equal($"Rating {rating} must be between 1 and 5", exception.Message);
    }

    private static ReviewInformation CreateReviewInformation(
        string? title = "One of the greatest sci-fi movie",
        string? content = "Denis Villeneuve has crafted a breathtaking sci-fi masterpiece. The cinematography and sweeping desert visuals are stunning, while the incredible cast—especially Timothée Chalamet and Zendaya—delivers deeply emotional performances. An absolute must-watch on the biggest screen possible!",
        int? rating = 5)
    {
        return ReviewInformation.Create(title, content, rating);
    }
}

