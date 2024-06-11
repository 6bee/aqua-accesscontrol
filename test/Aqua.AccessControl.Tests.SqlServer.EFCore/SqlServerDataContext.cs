// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SqlServer.EFCore;

using Aqua.AccessControl.Tests.DataModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Linq;

public class SqlServerDataContext : DbContext
{
    private readonly string _connectionString;

    public SqlServerDataContext(string connectionString)
    {
        _connectionString = connectionString;
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);

        optionsBuilder.UseSqlServer(_connectionString);
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
}
