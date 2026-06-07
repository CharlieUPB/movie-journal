using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using MovieJournal.Application.Users.Responses;
using MovieJournal.Web.IntegrationTests.TestInfrastructure;

namespace MovieJournal.Web.IntegrationTests.Users;

public class UserEndpointsTests
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);

    [Fact]
    public async Task Register_ShouldReturnCreatedAndToken()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = new
        {
            DisplayName = "New User",
            Email = $"{Guid.NewGuid():N}@moviejournal.test",
            Password = "Demo123!"
        };

        var response = await client.PostAsJsonAsync("/api/users/register", request);
        var body = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        Assert.NotNull(body);
        Assert.NotEqual(Guid.Empty, body.UserId);
        Assert.Equal("New User", body.DisplayName);
        Assert.Equal(request.Email, body.Email);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task Login_ShouldReturnOkAndTokenForValidCredentials()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = new
        {
            Email = "demo@moviejournal.com",
            Password = "Demo123!"
        };

        var response = await client.PostAsJsonAsync("/api/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<UserResponse>(JsonOptions);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Demo User", body.DisplayName);
        Assert.Equal("demo@moviejournal.com", body.Email);
        Assert.False(string.IsNullOrWhiteSpace(body.Token));
    }

    [Fact]
    public async Task Login_ShouldReturnBadRequestProblemDetailsForInvalidCredentials()
    {
        using var factory = new MovieJournalWebApplicationFactory();
        using var client = factory.CreateClient();
        var request = new
        {
            Email = "demo@moviejournal.com",
            Password = "WrongPassword"
        };

        var response = await client.PostAsJsonAsync("/api/users/login", request);
        var body = await response.Content.ReadFromJsonAsync<ProblemDetailsResponseDto>(JsonOptions);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.NotNull(body);
        Assert.Equal("Use case error", body.Title);
        Assert.Equal("Invalid credentials", body.Detail);
    }

    private record ProblemDetailsResponseDto(
        string? Title,
        string? Detail,
        int? Status);
}
