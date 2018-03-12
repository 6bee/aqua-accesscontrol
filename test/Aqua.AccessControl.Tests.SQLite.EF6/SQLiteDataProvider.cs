// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EF6
{
    using Aqua.AccessControl.Tests.DataModel;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity;
    using System.Data.SQLite;
    using System.Linq;

    public class SQLiteDataProvider : DbContext, IDataProvider
    {
        private static string SQLiteConnectionString =>
            new SQLiteConnectionStringBuilder
            {
                DataSource = "|DataDirectory|sampledb.sqlite",
                ForeignKeys = true,
            }.ConnectionString;

        public SQLiteDataProvider()
            : base(new SQLiteConnection(SQLiteConnectionString), true)
        {
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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