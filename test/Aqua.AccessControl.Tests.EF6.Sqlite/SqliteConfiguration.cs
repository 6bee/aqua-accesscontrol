// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.EF6.Sqlite;

using System.Data.Entity;
using System.Data.Entity.Core.Common;
using System.Data.SQLite;
using System.Data.SQLite.EF6;

public class SqliteConfiguration : DbConfiguration
{
    public SqliteConfiguration()
    {
        SetDatabaseInitializer(new SqliteDatabaseInitializer());
        SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
        SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
        SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
    }
}