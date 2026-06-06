using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

namespace MovieJournal.Infrastructure.persistence
{
    public interface ISqlConnectionFactory
    {
        IDbConnection GetOpenConnection();

        IDbConnection CreateNewConnection();

        string GetConnectionString();
    }
}
