// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EFCore;

using Aqua.AccessControl.Tests.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

public class SQLiteDataContext : DbContext
{
    private readonly string _connectionString;

    public SQLiteDataContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder
            .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking)
            .UseSqlite(_connectionString);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        var tenantModel = modelBuilder.Entity<Tenant>();

        var claimModel = modelBuilder.Entity<Claim>();

        var productCategoryModel = modelBuilder.Entity<ProductCategory>();

        var productModel = modelBuilder.Entity<Product>();
        productModel
            .HasOne(x => x.ProductCategory)
            .WithMany()
            .HasForeignKey("ProductCategoryId");

        var orderModel = modelBuilder.Entity<Order>();
        orderModel
            .HasMany(x => x.Items)
            .WithOne()
            .HasForeignKey(x => x.OrderId)
            .IsRequired(true);

        var orderItemModel = modelBuilder.Entity<OrderItem>();

        var parentModel = modelBuilder.Entity<Parent>();
        parentModel
            .HasMany(x => x.Children)
            .WithOne()
            .HasForeignKey("ParentId");
        var childModel = modelBuilder.Entity<Child>();
        childModel
            .HasOne(x => x.Parent)
            .WithMany()
            .HasForeignKey("ParentId");
        childModel
            .HasOne(x => x.Self)
            .WithOne()
            .HasForeignKey(typeof(Child), nameof(Child.Id));

        var pkProperty = typeof(Entity).GetProperty(nameof(Entity.Id));
        var primaryKeys = (
            from type in modelBuilder.Model.GetEntityTypes()
            from property in type.GetProperties()
            where property.PropertyInfo?.Name == pkProperty.Name
               && property.PropertyInfo.DeclaringType == pkProperty.DeclaringType
            select property)
            .ToArray();
        foreach (var k in primaryKeys)
        {
            k.ValueGenerated = ValueGenerated.Never;
        }
    }

    public virtual DbSet<Tenant> Tenants { get; set; }

    public virtual DbSet<Claim> Claims { get; set; }

    public virtual DbSet<Product> Products { get; set; }

    public virtual DbSet<ProductCategory> ProductCategories { get; set; }

    public virtual DbSet<Order> Orders { get; set; }

    public virtual DbSet<Parent> Parents { get; set; }

    public virtual DbSet<Child> Children { get; set; }
}
