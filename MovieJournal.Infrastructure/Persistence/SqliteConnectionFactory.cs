using Microsoft.Data.Sqlite;
using System.Data;

namespace MovieJournal.Infrastructure.persistence;

public class SqliteConnectionFactory : ISqlConnectionFactory, IDisposable
{

    private readonly string _connectionString;
    private IDbConnection? _connection;

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
        if (_connection == null || _connection.State != ConnectionState.Open)
        {
            _connection = new SqliteConnection(_connectionString);
            _connection.Open();
        }

        return _connection;
    }

    public void Dispose()
    {
        if (_connection != null && _connection.State == ConnectionState.Open)
        {
            _connection.Close();
            _connection.Dispose();
        }
    }

}

