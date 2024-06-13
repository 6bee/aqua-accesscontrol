// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.EFCore.Sqlite;

using Aqua.AccessControl.Tests.DataModel;
using System;
using System.Linq;

public class SQLiteDataProvider : IDataProvider
{
    private readonly SQLiteDataContext _dataContext;

    public SQLiteDataProvider()
    {
        var connectionString = $"DataSource=testdb-{Guid.NewGuid()}.sqlite;";
        _dataContext = new SQLiteDataContext(connectionString);
        var created = _dataContext.Database.EnsureCreated();
        if (created)
        {
            new SQLiteDataSeeder().Seed(_dataContext);
        }
    }

    public void Dispose()
    {
        _dataContext.Database.EnsureDeleted();
        _dataContext.Dispose();
    }

    public IQueryable<Tenant> Tenants => _dataContext.Tenants;

    public IQueryable<Claim> Claims => _dataContext.Claims;

    public IQueryable<ProductCategory> ProductCategories => _dataContext.ProductCategories;

    public IQueryable<Product> Products => _dataContext.Products;

    public IQueryable<Order> Orders => _dataContext.Orders;

    public IQueryable<Parent> Parents => _dataContext.Parents;

    public IQueryable<Child> Children => _dataContext.Children;
}