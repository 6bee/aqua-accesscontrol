// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests.SQLite.EF6
{
    using Aqua.AccessControl.Tests.DataModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Common;
    using System.Data.Entity;
    using System.Data.Entity.Core.Common;
    using System.Data.SQLite;
    using System.Data.SQLite.EF6;
    using System.Linq;

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

    public class SQLiteDatabaseInitializer : IDatabaseInitializer<SQLiteDataProvider>
    {
        private const string ddl = @"
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
            using (var source = new InMemoryDataProvider())
            {
                context.Tenants.AddRange(source.Tenants);
                context.SaveChanges();
                context.Claims.AddRange(source.Claims);
                context.SaveChanges();
                context.ProductCategories.AddRange(source.ProductCategories);
                context.SaveChanges();
                context.Products.AddRange(source.Products);
                context.SaveChanges();
                context.Orders.AddRange(source.Orders);

                context.SaveChanges();
            }
        }

        private static void InitializeDatabaseObjects(SQLiteDataProvider context)
        {
            var connection = context.Database.Connection;
            connection.Open();
            ExecuteNonQuery(connection, ddl);
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

    public class SQLiteDataProvider : DbContext, IDataProvider
    {
        private static string SQLiteConnectionString =>
            new SQLiteConnectionStringBuilder
            {
                DataSource = "|DataDirectory|sampledb.sqlite",
                ForeignKeys = true
            }.ConnectionString;

        public SQLiteDataProvider() 
            : base(new SQLiteConnection(SQLiteConnectionString), true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //modelBuilder.Conventions
            //    .Remove<PluralizingTableNameConvention>();

            modelBuilder
                .Types<Entity>()
                .Configure(c => c
                    .Property(x => x.Id)
                    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None));

            var tenantModel = modelBuilder.Entity<Tenant>();

            var claimModel = modelBuilder.Entity<Claim>();

            var productCategoryModel = modelBuilder.Entity<ProductCategory>();

            var productModel = modelBuilder.Entity<Product>();
            productModel
                .HasRequired(x => x.ProductCategory)
                .WithMany()
                .Map(m => m.MapKey("ProductCategoryId"));

            var orderModel = modelBuilder.Entity<Order>();
            orderModel
                .HasMany(x => x.Items)
                .WithRequired();

            var orderItemModel = modelBuilder.Entity<OrderItem>();
        }

        public virtual DbSet<Tenant> Tenants { get; set; }
        public virtual DbSet<Claim> Claims { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductCategory> ProductCategories { get; set; }
        public virtual DbSet<Order> Orders { get; set; }

        IQueryable<Tenant> IDataProvider.Tenants => Tenants;
        IQueryable<Claim> IDataProvider.Claims => Claims;
        IQueryable<ProductCategory> IDataProvider.ProductCategories => ProductCategories;
        IQueryable<Product> IDataProvider.Products => Products;
        IQueryable<Order> IDataProvider.Orders => Orders;
    }
}