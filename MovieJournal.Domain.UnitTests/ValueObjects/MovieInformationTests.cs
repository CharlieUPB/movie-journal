using MovieJournal.Domain.Exceptions;
using MovieJournal.Domain.ValueObjects;

namespace MovieJournal.Domain.UnitTests.ValueObjects;

public class MovieInformationTests
{
    private static readonly DateOnly Today = DateOnly.FromDateTime(DateTime.Today);

    [Fact]
    public void ShouldSaveMovieInformation()
    {
        var title = "Project Hail Mary";
        var releaseYear = 2025;

        var movieInformation = CreateMovieInformation(title, releaseYear);

        Assert.Equal(title, movieInformation.MovieTitle);
        Assert.Equal(releaseYear, movieInformation.ReleaseYear);
    }

    [Fact]
    public void ShouldSaveMovieTitleWithoutReleaseYear()
    {
        var title = "Project Hail Mary";

        var movieInformation = CreateMovieInformation(title);

        Assert.Equal(title, movieInformation.MovieTitle);
        Assert.Null(movieInformation.ReleaseYear);
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("      ")]
    public void ShouldThrowDomainExceptionWhenMovieTitleIsInvalid(string? title)
    {
        var exception = Assert.Throws<DomainException>(() =>
            CreateMovieInformation(title));

        Assert.Equal("Movie title is required", exception.Message);
    }

    [Fact]
    public void ShouldThrowDomainExceptionWhenReleaseYearIsInTheFuture()
    {
        var futureYear = Today.AddYears(1).Year;

        var exception = Assert.Throws<DomainException>(() =>
            CreateMovieInformation(releaseYear: futureYear));

        Assert.Equal("Release year cannot be in the future.", exception.Message);
    }

    private static MovieInformation CreateMovieInformation(
        string? title = "Project Hail Mary",
        int? releaseYear = null)
    {
        return MovieInformation.Create(title, Today, releaseYear);
    }
}
