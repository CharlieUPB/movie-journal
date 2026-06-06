using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MovieJournal.Infrastructure.Persistence.Connection
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();

        IDbConnection CreateNewConnection();

        string GetConnectionString();
    }
}
