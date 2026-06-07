using Microsoft.Data.Sqlite;

namespace MovieJournal.Web.IntegrationTests.TestInfrastructure;

internal sealed class TestDatabase : IDisposable
{
    public TestDatabase()
    {
        DatabasePath = Path.Combine(Path.GetTempPath(), $"movie-journal-web-{Guid.NewGuid():N}.db");
        ConnectionString = $"Data Source={DatabasePath};Pooling=False";
    }

    public string DatabasePath { get; }
    public string ConnectionString { get; }

    public void Dispose()
    {
        SqliteConnection.ClearAllPools();

        if (File.Exists(DatabasePath))
        {
            File.Delete(DatabasePath);
        }
    }
}
