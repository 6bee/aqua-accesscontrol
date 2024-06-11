﻿// <auto-generated />
using Aqua.AccessControl.Tests.SQLite.EFCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using System;

namespace Aqua.AccessControl.Tests.SQLite.EFCore.Migrations;

[DbContext(typeof(SQLiteDataProvider))]
partial class SQLiteDataProviderModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
#pragma warning disable 612, 618
        modelBuilder
            .HasAnnotation("ProductVersion", "2.0.1-rtm-125");

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.Claim", b =>
            {
                b.Property<long>("Id");

                b.Property<string>("Subject");

                b.Property<int>("TenantId");

                b.Property<string>("Type");

                b.Property<string>("Value");

                b.HasKey("Id");

                b.ToTable("Claims");
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.Order", b =>
            {
                b.Property<long>("Id");

                b.Property<int>("TenantId");

                b.HasKey("Id");

                b.ToTable("Orders");
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.OrderItem", b =>
            {
                b.Property<long>("Id");

                b.Property<long>("OrderId");

                b.Property<decimal>("Price");

                b.Property<long>("ProductId");

                b.Property<int>("Quantity");

                b.HasKey("Id");

                b.HasIndex("OrderId");

                b.ToTable("OrderItem");
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.Product", b =>
            {
                b.Property<long>("Id");

                b.Property<string>("Name");

                b.Property<decimal>("Price");

                b.Property<long?>("ProductCategoryId");

                b.Property<int>("TenantId");

                b.HasKey("Id");

                b.HasIndex("ProductCategoryId");

                b.ToTable("Products");
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.ProductCategory", b =>
            {
                b.Property<long>("Id");

                b.Property<string>("Name");

                b.Property<int>("TenantId");

                b.HasKey("Id");

                b.ToTable("ProductCategories");
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.Tenant", b =>
            {
                b.Property<long>("Id");

                b.Property<string>("Name");

                b.HasKey("Id");

                b.ToTable("Tenants");
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.OrderItem", b =>
            {
                b.HasOne("Aqua.AccessControl.Tests.DataModel.Order")
                    .WithMany("Items")
                    .HasForeignKey("OrderId")
                    .OnDelete(DeleteBehavior.Cascade);
            });

        modelBuilder.Entity("Aqua.AccessControl.Tests.DataModel.Product", b =>
            {
                b.HasOne("Aqua.AccessControl.Tests.DataModel.ProductCategory", "ProductCategory")
                    .WithMany()
                    .HasForeignKey("ProductCategoryId");
            });
#pragma warning restore 612, 618
    }
}
