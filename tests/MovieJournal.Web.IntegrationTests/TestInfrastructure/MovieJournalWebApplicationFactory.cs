using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace MovieJournal.Web.IntegrationTests.TestInfrastructure;

internal sealed class MovieJournalWebApplicationFactory : WebApplicationFactory<Program>
{
    public MovieJournalWebApplicationFactory()
    {
        Database = new TestDatabase();
    }

    public TestDatabase Database { get; }

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
}
