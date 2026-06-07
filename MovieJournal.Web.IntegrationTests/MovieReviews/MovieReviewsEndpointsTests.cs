using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieJournal.Web.IntegrationTests.TestInfrastructure;

namespace MovieJournal.Web.IntegrationTests.MovieReviews;

public class MovieReviewsEndpointsTests
{
    private static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly Guid SeededPublishedReviewId = Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa");
    private static readonly Guid SeededDraftReviewId = Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb");
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ShouldReturnOkAndListAllMovieReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movie-reviews");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotNull(body.Reviews);
        Assert.NotEmpty(body.Reviews);
    }

    [Fact]
    public async Task ShouldReturnOkAndOnlyPublishedReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movie-reviews/published");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Reviews);
        Assert.All(body.Reviews, review => Assert.Equal("Published", review.Status));
    }

    [Fact]
    public async Task ShouldReturnOkAndOnlyCurrentUserReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movie-reviews/my");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Reviews);
        Assert.All(body.Reviews, review => Assert.Equal(DemoUserId, review.UserId));
    }

    [Fact]
    public async Task ShouldReturnOkAndOnlyCurrentUserDraftReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movie-reviews/my?status=Draft");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Reviews);
        Assert.All(body.Reviews, review =>
        {
            Assert.Equal(DemoUserId, review.UserId);
            Assert.Equal("Draft", review.Status);
        });
    }

    [Fact]
    public async Task ShouldReturnOkWhenReviewExistsAndIsViewable()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/movie-reviews/{SeededPublishedReviewId}");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(SeededPublishedReviewId, body.Id);
    }

    [Fact]
    public async Task ShouldReturnBadRequestProblemDetailsWhenReviewDoesNotExist()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync($"/api/movie-reviews/{Guid.NewGuid()}");
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Use case error", body.Title);
        Assert.Equal("Movie review was not found", body.Detail);
    }

    [Fact]
    public async Task ShouldCreateMovieReview()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = CreateMovieReviewRequest();

        var response = await client.PostAsJsonAsync("/api/movie-reviews", request);
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(response.Headers.Location);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.Id);
        Assert.Equal("Dune: Part Two", body.MovieTitle);
        Assert.Equal("One of the greatest sci-fi movies", body.ReviewTitle);
        Assert.Equal(5, body.ReviewRating);
        Assert.Equal("Draft", body.Status);
    }

    [Fact]
    public async Task ShouldReturnBadRequestProblemDetailsWhenReviewContentIsTooShort()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = CreateMovieReviewRequest(reviewContent: "Too short");

        var response = await client.PostAsJsonAsync("/api/movie-reviews", request);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Domain validation error", body.Title);
        Assert.Contains("Content is required and must be minimum of 50 characters", body.Detail);
    }

    [Fact]
    public async Task ShouldUpdateMovieReview()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);
        var updateRequest = new
        {
            MovieTitle = "Updated title",
            ReviewTitle = "Updated review title",
            ReviewContent = "This is valid updated review content longer than fifty characters for the API integration test.",
            ReviewRating = 4,
            MovieReleaseYear = 2023
        };

        var response = await client.PutAsJsonAsync($"/api/movie-reviews/{createdReview!.Id}", updateRequest);
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Updated title", body.MovieTitle);
        Assert.Equal("Updated review title", body.ReviewTitle);
        Assert.Equal(updateRequest.ReviewContent, body.ReviewContent);
        Assert.Equal(4, body.ReviewRating);
        Assert.Equal(2023, body.MovieReleaseYear);
    }

    [Fact]
    public async Task ShouldDeleteMovieReview()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        var deleteResponse = await client.DeleteAsync($"/api/movie-reviews/{createdReview!.Id}");
        var getResponse = await client.GetAsync($"/api/movie-reviews/{createdReview.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeletingSameReviewTwice()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        var firstDeleteResponse = await client.DeleteAsync($"/api/movie-reviews/{createdReview!.Id}");
        var secondDeleteResponse = await client.DeleteAsync($"/api/movie-reviews/{createdReview.Id}");

        Assert.Equal(HttpStatusCode.NoContent, firstDeleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task PublishMovieReview_ShouldReturnOkForDraftReviewOwnedByCurrentUser()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/movie-reviews/{SeededDraftReviewId}/publish", null);
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Published", body.Status);
    }

    [Fact]
    public async Task PublishMovieReview_ShouldReturnBadRequestProblemDetailsWhenReviewDoesNotExist()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/movie-reviews/{Guid.NewGuid()}/publish", null);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains("Movie review was not found", body.Detail);
    }

    [Fact]
    public async Task PublishMovieReview_ShouldReturnBadRequestProblemDetailsWhenReviewIsAlreadyPublished()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/movie-reviews/{SeededPublishedReviewId}/publish", null);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains("Only draft reviews can be published", body.Detail);
    }

    [Fact]
    public async Task ArchiveMovieReview_ShouldReturnOkForReviewOwnedByCurrentUser()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/movie-reviews/{SeededPublishedReviewId}/archive", null);
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Archived", body.Status);
    }

    [Fact]
    public async Task ArchiveMovieReview_ShouldReturnBadRequestProblemDetailsWhenReviewDoesNotExist()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.PostAsync($"/api/movie-reviews/{Guid.NewGuid()}/archive", null);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Contains("Movie review was not found", body.Detail);
    }

    [Fact]
    public async Task ArchiveMovieReview_ShouldBeSafeForAlreadyArchivedReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        var firstArchiveResponse = await client.PostAsync($"/api/movie-reviews/{createdReview!.Id}/archive", null);
        var firstArchiveBody = await firstArchiveResponse.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);
        var secondArchiveResponse = await client.PostAsync($"/api/movie-reviews/{createdReview.Id}/archive", null);
        var secondArchiveBody = await secondArchiveResponse.Content.ReadFromJsonAsync<MovieReviewResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, firstArchiveResponse.StatusCode);
        Assert.Equal("Archived", firstArchiveBody?.Status);
        Assert.Equal(HttpStatusCode.OK, secondArchiveResponse.StatusCode);
        Assert.Equal("Archived", secondArchiveBody?.Status);
    }

    private static object CreateMovieReviewRequest(string? reviewContent = null)
    {
        return new
        {
            MovieTitle = "Dune: Part Two",
            ReviewTitle = "One of the greatest sci-fi movies",
            ReviewContent = reviewContent ?? "This is valid review content longer than fifty characters for the API integration test.",
            ReviewRating = 5,
            MovieReleaseYear = 2024
        };
    }

    private sealed record MovieReviewsListResponseDto(IReadOnlyList<MovieReviewResponseDto> Reviews);

    private sealed record MovieReviewResponseDto(
        Guid Id,
        Guid UserId,
        string MovieTitle,
        int? MovieReleaseYear,
        string ReviewTitle,
        string ReviewContent,
        int ReviewRating,
        string Status,
        DateTime CreatedAt,
        DateTime? UpdatedAt);

    private sealed record ProblemDetailsResponseDto(
        string? Title,
        string? Detail,
        int? Status);
}
