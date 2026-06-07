using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace MovieJournal.Web.IntegrationTests.TestInfrastructure;

internal sealed class MovieJournalWebApplicationFactory : WebApplicationFactory<Program>
{
    public MovieJournalWebApplicationFactory()
    {
        Database = new TestDatabase();
    }

    public TestDatabase Database { get; }

    public async Task<HttpClient> CreateAuthenticatedClientAsync()
    {
        var client = CreateClient();

        var response = await client.PostAsJsonAsync(
            "/api/users/login",
            new
            {
                Email = "demo@moviejournal.com",
                Password = "Demo123!"
            });

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<AuthResponseDto>();

        if (body is null || string.IsNullOrWhiteSpace(body.Token))
        {
            throw new InvalidOperationException("The test login response did not include a token.");
        }

        client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", body.Token);

        return client;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configurationBuilder) =>
        {
            configurationBuilder.AddInMemoryCollection(
                new Dictionary<string, string?>
                {
                    ["ConnectionStrings:MovieJournalDb"] = Database.ConnectionString
                });
        });
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            Database.Dispose();
        }
    }

    private record AuthResponseDto(string Token);
}
