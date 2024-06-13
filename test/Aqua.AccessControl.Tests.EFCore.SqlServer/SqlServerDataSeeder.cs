// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.EFCore.SqlServer;

public class SqlServerDataSeeder
{
    public void Seed(SqlServerDataContext context)
    {
        using var source = new InMemoryDataProvider();

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

        context.Parents.AddRange(source.Parents);
        context.Children.AddRange(source.Children);
        context.SaveChanges();
    }
}