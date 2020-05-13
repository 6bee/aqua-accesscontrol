// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EF6
{
    using System.Data.Common;
    using System.Data.Entity;
    using System.Linq;

    public class SQLiteDatabaseInitializer : IDatabaseInitializer<SQLiteDataProvider>
    {
        private const string Ddl = @"
DROP TABLE IF EXISTS [OrderItems];
DROP TABLE IF EXISTS [Orders];
DROP TABLE IF EXISTS [Products];
DROP TABLE IF EXISTS [ProductCategories];
DROP TABLE IF EXISTS [Claims];
DROP TABLE IF EXISTS [Tenants];


CREATE TABLE IF NOT EXISTS [Tenants](
    [Id] INTEGER NOT NULL PRIMARY KEY,
    [Name] TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS [Claims](
    [Id] INTEGER NOT NULL PRIMARY KEY,
	[TenantId] INTEGER NULL,
    [Type] TEXT NOT NULL,
    [Value] TEXT NOT NULL,
    [Subject] TEXT NOT NULL,
	FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id])
);

CREATE TABLE IF NOT EXISTS [ProductCategories](
    [Id] INTEGER NOT NULL PRIMARY KEY,
	[TenantId] INTEGER NOT NULL,
    [Name] TEXT NOT NULL,
	FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id])
);

CREATE TABLE IF NOT EXISTS [Products](
    [Id] INTEGER NOT NULL PRIMARY KEY,
	[TenantId] INTEGER NOT NULL,
    [ProductCategoryId] INTEGER NOT NULL,
    [Name] TEXT NOT NULL,
    [Price] NUMBER NOT NULL,
	FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id]),
    FOREIGN KEY ([ProductCategoryId]) REFERENCES [ProductCategories]([Id])
);

CREATE TABLE IF NOT EXISTS [Orders](
    [Id] INTEGER NOT NULL PRIMARY KEY,
	[TenantId] INTEGER NOT NULL,
	FOREIGN KEY ([TenantId]) REFERENCES [Tenants]([Id])
);

CREATE TABLE IF NOT EXISTS [OrderItems](
    [Id] INTEGER NOT NULL PRIMARY KEY,
    [OrderId] INTEGER NOT NULL,
    [ProductId] INTEGER NOT NULL,
    [Quantity] INTEGER NOT NULL,
    [Price] NUMBER NOT NULL,
    FOREIGN KEY ([OrderId]) REFERENCES [Orders]([Id]),
    FOREIGN KEY ([ProductId]) REFERENCES [Products]([Id])
);
";

        public void InitializeDatabase(SQLiteDataProvider context)
        {
            InitializeDatabaseObjects(context);
            InitializeDataRecords(context);
        }

        private void InitializeDataRecords(SQLiteDataProvider context)
        {
            void Add<T>(IQueryable<T> source) where T : class
            {
                context.Set<T>().AddRange(source);
                context.SaveChanges();
            }

            using var source = new InMemoryDataProvider();
            Add(source.Tenants);
            Add(source.Claims);
            Add(source.ProductCategories);
            Add(source.Products);
            Add(source.Orders);
        }

        private static void InitializeDatabaseObjects(SQLiteDataProvider context)
        {
            var connection = context.Database.Connection;
            connection.Open();
            ExecuteNonQuery(connection, Ddl);
            connection.Close();
        }

        private static int ExecuteNonQuery(DbConnection connection, string commandText)
        {
            var command = connection.CreateCommand();
            command.CommandText = commandText;
            var result = command.ExecuteNonQuery();
            return result;
        }
    }
}