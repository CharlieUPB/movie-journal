using Microsoft.Data.Sqlite;
using System.Data;

namespace MovieJournal.Infrastructure.Persistence.Connection;

public class SqliteConnectionFactory : ISqlConnectionFactory
{

    private readonly string _connectionString;

    public SqliteConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    public IDbConnection CreateNewConnection()
    {
        var connection = new SqliteConnection(_connectionString);

        connection.Open();

        return connection;
    }
    public string GetConnectionString()
    {
        return _connectionString;
    }

    public IDbConnection GetOpenConnection()
    {
        return CreateNewConnection();
    }
}

