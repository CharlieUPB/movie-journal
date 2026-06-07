using MovieJournal.Application.MovieReviews.Queries;
using MovieJournal.Application.MovieReviews.Requests;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Application.UnitTests.MovieReviews.Fakes;
using MovieJournal.Domain.Enums;

namespace MovieJournal.Application.UnitTests.MovieReviews.Queries;

public class ListPublishedMovieReviewsQueryTests
{
    [Fact]
    public async Task ShouldRequestOnlyPublishedReviewsThroughRepositoryCriteria()
    {
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Draft));
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published));
        var query = new ListPublishedMovieReviewsQuery(repository);

        var response = await query.Execute(new ListPublishedMovieReviewsRequest());

        Assert.Equal(ReviewStatus.Published, repository.LastCriteriaUsed?.Status);
        Assert.Single(response.Reviews);
        Assert.Equal("Published", response.Reviews[0].Status);
    }

    [Fact]
    public async Task ShouldReturnResponseDtos()
    {
        var repository = new FakeMovieReviewsRepository();
        repository.Reviews.Add(MovieReviewTestData.CreateMovieReview(status: ReviewStatus.Published));
        var query = new ListPublishedMovieReviewsQuery(repository);

        var response = await query.Execute(new ListPublishedMovieReviewsRequest());

        Assert.IsType<MovieReviewResponse>(response.Reviews[0]);
    }
}
