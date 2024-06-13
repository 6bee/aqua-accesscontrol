// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.EFCore.Sqlite;

using System.Linq;

public class SQLiteDataSeeder
{
    public void Seed(SQLiteDataContext context)
    {
        void Add<T>(IQueryable<T> source)
            where T : class
            => context.Set<T>().AddRange(source);

        using var source = new InMemoryDataProvider();

        Add(source.Tenants);
        Add(source.Claims);
        Add(source.ProductCategories);
        Add(source.Products);
        Add(source.Orders);
        Add(source.Parents);
        Add(source.Children);

        context.SaveChanges();
    }
}