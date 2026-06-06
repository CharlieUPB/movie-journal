using MovieJournal.Infrastructure.Persistence.Initializer;

namespace MovieJournal.Web;

public static class WebAppExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();

        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();

        await initializer.InitializeAsync();
    }
}