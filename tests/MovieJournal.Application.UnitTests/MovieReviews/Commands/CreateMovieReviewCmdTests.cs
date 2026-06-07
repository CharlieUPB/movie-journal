using MovieJournal.Application.MovieReviews.Commands;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Entities;

namespace MovieJournal.Application.UnitTests.MovieReviews.Commands;

public class CreateMovieReviewCmdTests
{
    [Fact]
    public async Task ShouldCreateMovieReviewSuccessfully()
    {
        var repository = new FakeMovieReviewsRepository();
        var command = new CreateMovieReviewCmd(repository);
        var userId = Guid.NewGuid();
        var request = CreateRequest(userId);

        var response = await command.Execute(request);

        Assert.Equal(1, repository.CreateAsyncCallCount);
        Assert.Single(repository.CreatedEntities);
        Assert.IsType<MovieReview>(repository.CreatedEntities[0]);
        Assert.Equal(userId, response.UserId);
        Assert.Equal("Project Hail Mary", response.MovieTitle);
        Assert.Equal("Great story", response.ReviewTitle);
        Assert.Equal(5, response.ReviewRating);
        Assert.Equal("Draft", response.Status);
    }

    [Fact]
    public async Task ShouldAllowOptionalMovieReleaseYear()
    {
        var repository = new FakeMovieReviewsRepository();
        var command = new CreateMovieReviewCmd(repository);
        var request = CreateRequest(Guid.NewGuid(), movieReleaseYear: null);

        var response = await command.Execute(request);

        Assert.Null(response.MovieReleaseYear);
        Assert.Null(repository.CreatedEntities[0].MovieInformation.ReleaseYear);
    }

    private static CreateMovieRequest CreateRequest(Guid userId, int? movieReleaseYear = 2026)
    {
        return new CreateMovieRequest(
            userId,
            "Project Hail Mary",
            "Great story",
            MovieReviewTestData.ValidReviewContent,
            5,
            movieReleaseYear);
    }
}
