// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EF6
{
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.SQLite;
    using System.Data.SQLite.EF6;

    public class SQLiteConfiguration : DbConfiguration
    {
        public SQLiteConfiguration()
        {
            SetDatabaseInitializer(new SQLiteDatabaseInitializer());
            SetProviderFactory("System.Data.SQLite", SQLiteFactory.Instance);
            SetProviderFactory("System.Data.SQLite.EF6", SQLiteProviderFactory.Instance);
            SetProviderServices("System.Data.SQLite", (DbProviderServices)SQLiteProviderFactory.Instance.GetService(typeof(DbProviderServices)));
        }
    }
}