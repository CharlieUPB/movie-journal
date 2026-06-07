using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieJournal.Application.MovieReviews.Responses;
using MovieJournal.Application.ReviewComments.Responses;
using MovieJournal.Web.IntegrationTests.TestInfrastructure;

namespace MovieJournal.Web.IntegrationTests.ReviewComments;

public class ReviewCommentsEndpointsTests
{
    private static readonly Guid DemoUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task ShouldReturnOkAndCommentsForPublishedReview()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreatePublishedReviewAsync(client);
        var comment = await CreateCommentAsync(client, review.Id, "This is a visible comment.");

        var response = await client.GetAsync($"/api/movie-reviews/{review.Id}/comments");
        var body = await response.Content.ReadFromJsonAsync<ReviewCommentsListResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        var returnedComment = Assert.Single(body.Comments);
        Assert.Equal(comment.Id, returnedComment.Id);
        Assert.Equal("This is a visible comment.", returnedComment.Content);
    }

    [Fact]
    public async Task ShouldCreateCommentForPublishedReview()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreatePublishedReviewAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/movie-reviews/{review.Id}/comments",
            CreateCommentRequest("This comment should be created."));
        var body = await response.Content.ReadFromJsonAsync<ReviewCommentResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("This comment should be created.", body.Content);
        Assert.Equal(review.Id, body.MovieReviewId);
        Assert.Equal(DemoUserId, body.UserId);
    }

    [Fact]
    public async Task ShouldReturnBadRequestProblemDetailsWhenCommentContentIsInvalid()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreatePublishedReviewAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/movie-reviews/{review.Id}/comments",
            CreateCommentRequest("  "));
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Domain validation error", body.Title);
        Assert.Contains("Comment content is requiered and should not exceed 500 chars", body.Detail);
    }

    [Fact]
    public async Task ShouldReturnBadRequestProblemDetailsWhenReviewIsDraft()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreateDraftReviewAsync(client);

        var response = await client.PostAsJsonAsync(
            $"/api/movie-reviews/{review.Id}/comments",
            CreateCommentRequest("This comment should be rejected."));
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Domain validation error", body.Title);
        Assert.Contains("Comments are allowed only on published reviews", body.Detail);
    }

    [Fact]
    public async Task ShouldUpdateCommentWhenUserIsAuthor()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreatePublishedReviewAsync(client);
        var comment = await CreateCommentAsync(client, review.Id, "Original comment");

        var response = await client.PutAsJsonAsync(
            $"/api/review-comments/{comment.Id}",
            CreateCommentRequest("Updated comment"));
        var body = await response.Content.ReadFromJsonAsync<ReviewCommentResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal(comment.Id, body.Id);
        Assert.Equal("Updated comment", body.Content);
    }

    [Fact]
    public async Task ShouldDeleteComment()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreatePublishedReviewAsync(client);
        var comment = await CreateCommentAsync(client, review.Id, "Comment to delete");

        var deleteResponse = await client.DeleteAsync($"/api/review-comments/{comment.Id}");
        var getResponse = await client.GetAsync($"/api/movie-reviews/{review.Id}/comments");
        var body = await getResponse.Content.ReadFromJsonAsync<ReviewCommentsListResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        Assert.NotNull(body);
        Assert.DoesNotContain(body.Comments, reviewComment => reviewComment.Id == comment.Id);
    }

    [Fact]
    public async Task ShouldReturnNoContentWhenDeletingSameCommentTwice()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var review = await CreatePublishedReviewAsync(client);
        var comment = await CreateCommentAsync(client, review.Id, "Comment to delete twice");

        var firstDeleteResponse = await client.DeleteAsync($"/api/review-comments/{comment.Id}");
        var secondDeleteResponse = await client.DeleteAsync($"/api/review-comments/{comment.Id}");

        Assert.Equal(HttpStatusCode.NoContent, firstDeleteResponse.StatusCode);
        Assert.Equal(HttpStatusCode.NoContent, secondDeleteResponse.StatusCode);
    }

    private static async Task<MovieReviewResponse> CreateDraftReviewAsync(HttpClient client)
    {
        var response = await client.PostAsJsonAsync("/api/movie-reviews", CreateMovieReviewRequest());
        var body = await response.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);

        return body;
    }

    private static async Task<MovieReviewResponse> CreatePublishedReviewAsync(HttpClient client)
    {
        var draftReview = await CreateDraftReviewAsync(client);

        var publishResponse = await client.PostAsync(
            $"/api/movie-reviews/{draftReview.Id}/publish",
            null);
        var publishedReview = await publishResponse.Content.ReadFromJsonAsync<MovieReviewResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, publishResponse.StatusCode);
        Assert.NotNull(publishedReview);
        Assert.Equal("Published", publishedReview.Status);

        return publishedReview;
    }

    private static async Task<ReviewCommentResponse> CreateCommentAsync(
        HttpClient client,
        Guid movieReviewId,
        string content)
    {
        var response = await client.PostAsJsonAsync(
            $"/api/movie-reviews/{movieReviewId}/comments",
            CreateCommentRequest(content));
        var body = await response.Content.ReadFromJsonAsync<ReviewCommentResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);

        return body;
    }

    private static object CreateMovieReviewRequest()
    {
        return new
        {
            MovieTitle = $"Comment Test Movie {Guid.NewGuid():N}",
            ReviewTitle = "One of the greatest sci-fi movies",
            ReviewContent = "This is valid review content longer than fifty characters for the API integration test.",
            ReviewRating = 5,
            MovieReleaseYear = 2024
        };
    }

    private static object CreateCommentRequest(string? content)
    {
        return new
        {
            Content = content
        };
    }

    private record ProblemDetailsResponseDto(
        string? Title,
        string? Detail,
        int? Status);
}
