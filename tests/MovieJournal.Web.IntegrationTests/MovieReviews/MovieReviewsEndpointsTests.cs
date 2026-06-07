using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Web.IntegrationTests.TestInfrastructure;

namespace MovieJournal.Web.IntegrationTests.MovieReviews;

public class MovieReviewsEndpointsTests
{
    private static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ShouldReturnOkAndListAllMovieReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();

        var response = await client.GetAsync("/api/movie-reviews");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponse>(JsonOptions);

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
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Reviews);
        Assert.All(body.Reviews, review => Assert.Equal("Published", review.Status));
    }

    [Fact]
    public async Task ShouldReturnOkAndOnlyCurrentUserReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/movie-reviews/my");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEmpty(body.Reviews);
        Assert.All(body.Reviews, review => Assert.Equal(DemoUserId, review.UserId));
    }

    [Fact]
    public async Task ShouldReturnOkAndOnlyCurrentUserDraftReviews()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();

        var response = await client.GetAsync("/api/movie-reviews/my?status=Draft");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewsListResponse>(JsonOptions);

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
        using var client = await factory.CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/movie-reviews",
            CreateMovieReviewRequest(movieTitle: $"Viewable Movie {Guid.NewGuid():N}"));

        var createdReview = await createResponse.Content
            .ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdReview);

        var response = await client.GetAsync($"/api/movie-reviews/{createdReview.Id}");
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(createdReview.Id, body.Id);
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
        using var client = await factory.CreateAuthenticatedClientAsync();
        var request = CreateMovieReviewRequest("Dune: Part Two", "One of the greatest sci-fi movies");

        var response = await client.PostAsJsonAsync("/api/movie-reviews", request);
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

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
        using var client = await factory.CreateAuthenticatedClientAsync();
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
        using var client = await factory.CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);
        var updateRequest = new
        {
            MovieTitle = "Updated title",
            ReviewTitle = "Updated review title",
            ReviewContent = "This is valid updated review content longer than fifty characters for the API integration test.",
            ReviewRating = 4,
            MovieReleaseYear = 2023
        };

        var response = await client.PutAsJsonAsync($"/api/movie-reviews/{createdReview!.Id}", updateRequest);
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

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
        using var client = await factory.CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        var deleteResponse = await client.DeleteAsync($"/api/movie-reviews/{createdReview!.Id}");
        var getResponse = await client.GetAsync($"/api/movie-reviews/{createdReview.Id}");

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.BadRequest, getResponse.StatusCode);
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeletingSameReviewTwice()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        var firstDeleteResponse = await client.DeleteAsync($"/api/movie-reviews/{createdReview!.Id}");
        var secondDeleteResponse = await client.DeleteAsync($"/api/movie-reviews/{createdReview.Id}");

        Assert.Equal(HttpStatusCode.NoContent, firstDeleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondDeleteResponse.StatusCode);
    }

    [Fact]
    public async Task PublishMovieReview_ShouldReturnOkForDraftReviewOwnedByCurrentUser()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/movie-reviews",
            CreateMovieReviewRequest(movieTitle: $"Draft Movie {Guid.NewGuid():N}"));

        var createdReview = await createResponse.Content
            .ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdReview);
        Assert.Equal("Draft", createdReview.Status);

        var response = await client.PostAsync(
            $"/api/movie-reviews/{createdReview.Id}/publish",
            null);

        var body = await response.Content
            .ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(createdReview.Id, body.Id);
        Assert.Equal("Published", body.Status);
    }

    [Fact]
    public async Task PublishMovieReview_ShouldReturnBadRequestProblemDetailsWhenReviewDoesNotExist()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();

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
        using var client = await factory.CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/movie-reviews",
            CreateMovieReviewRequest(movieTitle: $"Already Published Movie {Guid.NewGuid():N}"));

        var createdReview = await createResponse.Content
            .ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdReview);

        var firstPublishResponse = await client.PostAsync(
            $"/api/movie-reviews/{createdReview.Id}/publish",
            null);

        Assert.Equal(HttpStatusCode.OK, firstPublishResponse.StatusCode);

        var secondPublishResponse = await client.PostAsync(
            $"/api/movie-reviews/{createdReview.Id}/publish",
            null);

        var body = await secondPublishResponse.Content
            .ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, secondPublishResponse.StatusCode);
        Assert.NotNull(body);
        Assert.Contains("Only draft reviews can be published", body.Detail);
    }

    [Fact]
    public async Task ArchiveMovieReview_ShouldReturnOkForReviewOwnkedByCurrentUser()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();

        var createResponse = await client.PostAsJsonAsync(
            "/api/movie-reviews",
            CreateMovieReviewRequest(movieTitle: $"Archivable Movie {Guid.NewGuid():N}"));

        var createdReview = await createResponse.Content
            .ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);
        Assert.NotNull(createdReview);

        var response = await client.PostAsync(
            $"/api/movie-reviews/{createdReview.Id}/archive",
            null);

        var body = await response.Content
            .ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(createdReview.Id, body.Id);
        Assert.Equal("Archived", body.Status);
    }

    [Fact]
    public async Task ArchiveMovieReview_ShouldReturnBadRequestProblemDetailsWhenReviewDoesNotExist()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = await factory.CreateAuthenticatedClientAsync();

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
        using var client = await factory.CreateAuthenticatedClientAsync();
        var createResponse = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var createdReview = await createResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        var firstArchiveResponse = await client.PostAsync($"/api/movie-reviews/{createdReview!.Id}/archive", null);
        var firstArchiveBody = await firstArchiveResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);
        var secondArchiveResponse = await client.PostAsync($"/api/movie-reviews/{createdReview.Id}/archive", null);
        var secondArchiveBody = await secondArchiveResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, firstArchiveResponse.StatusCode);
        Assert.Equal("Archived", firstArchiveBody?.Status);
        Assert.Equal(HttpStatusCode.OK, secondArchiveResponse.StatusCode);
        Assert.Equal("Archived", secondArchiveBody?.Status);
    }

    private static object CreateMovieReviewRequest(string? movieTitle = null,
        string? reviewTitle = null,
        string? reviewContent = null,
        int reviewRating = 5,
        int? movieReleaseYear = 2024)
    {
        return new
        {
            MovieTitle = movieTitle ?? $"Test Movie {Guid.NewGuid():N}",
            ReviewTitle = reviewTitle ?? "One of the greatest sci-fi movies",
            ReviewContent = reviewContent ?? "This is valid review content longer than fifty characters for the API integration test.",
            ReviewRating = reviewRating,
            MovieReleaseYear = movieReleaseYear
        };
    }

    private record ProblemDetailsResponseDto(
        string? Title,
        string? Detail,
        int? Status);
}
