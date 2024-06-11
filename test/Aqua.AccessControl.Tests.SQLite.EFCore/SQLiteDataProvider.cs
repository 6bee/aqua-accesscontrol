// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EFCore;

using Aqua.AccessControl.Tests.DataModel;
using System.Linq;

public class SQLiteDataProvider : Disposable, IDataProvider
{
    private readonly SQLiteDataContext _dataContext;

    public SQLiteDataProvider()
    {
        _dataContext = new SQLiteDataContext();
        var created = _dataContext.Database.EnsureCreated();
        if (created)
        {
            new SQLiteDataSeeder().Seed(_dataContext);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && !Disposed)
        {
            _dataContext.Database.EnsureDeleted();
        }
    }

    public IQueryable<Tenant> Tenants => _dataContext.Tenants;

    public IQueryable<Claim> Claims => _dataContext.Claims;

    public IQueryable<ProductCategory> ProductCategories => _dataContext.ProductCategories;

    public IQueryable<Product> Products => _dataContext.Products;

    public IQueryable<Order> Orders => _dataContext.Orders;
}
