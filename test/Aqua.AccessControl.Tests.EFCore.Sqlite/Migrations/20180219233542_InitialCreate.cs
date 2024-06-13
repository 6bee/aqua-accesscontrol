// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EFCore.Migrations;

using Microsoft.EntityFrameworkCore.Migrations;

public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "Claims",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false),
                Subject = table.Column<string>(nullable: true),
                TenantId = table.Column<int>(nullable: false),
                Type = table.Column<string>(nullable: true),
                Value = table.Column<string>(nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Claims", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Orders",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false),
                TenantId = table.Column<int>(nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Orders", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "ProductCategories",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false),
                Name = table.Column<string>(nullable: true),
                TenantId = table.Column<int>(nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_ProductCategories", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "Tenants",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false),
                Name = table.Column<string>(nullable: true),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Tenants", x => x.Id);
            });

        migrationBuilder.CreateTable(
            name: "OrderItem",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false),
                OrderId = table.Column<long>(nullable: false),
                Price = table.Column<decimal>(nullable: false),
                ProductId = table.Column<long>(nullable: false),
                Quantity = table.Column<int>(nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_OrderItem", x => x.Id);
                table.ForeignKey(
                    name: "FK_OrderItem_Orders_OrderId",
                    column: x => x.OrderId,
                    principalTable: "Orders",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "Products",
            columns: table => new
            {
                Id = table.Column<long>(nullable: false),
                Name = table.Column<string>(nullable: true),
                Price = table.Column<decimal>(nullable: false),
                ProductCategoryId = table.Column<long>(nullable: true),
                TenantId = table.Column<int>(nullable: false),
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Products", x => x.Id);
                table.ForeignKey(
                    name: "FK_Products_ProductCategories_ProductCategoryId",
                    column: x => x.ProductCategoryId,
                    principalTable: "ProductCategories",
                    principalColumn: "Id",
                    onDelete: ReferentialAction.Restrict);
            });

        migrationBuilder.CreateIndex(
            name: "IX_OrderItem_OrderId",
            table: "OrderItem",
            column: "OrderId");

        migrationBuilder.CreateIndex(
            name: "IX_Products_ProductCategoryId",
            table: "Products",
            column: "ProductCategoryId");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Claims");

        migrationBuilder.DropTable(
            name: "OrderItem");

        migrationBuilder.DropTable(
            name: "Products");

        migrationBuilder.DropTable(
            name: "Tenants");

        migrationBuilder.DropTable(
            name: "Orders");

        migrationBuilder.DropTable(
            name: "ProductCategories");
    }
}