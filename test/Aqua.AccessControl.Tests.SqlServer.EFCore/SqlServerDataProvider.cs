// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SqlServer.EFCore;

using Aqua.AccessControl.Tests.DataModel;
using System;
using System.Linq;

public class SqlServerDataProvider : IDataProvider
{
    private readonly SqlServerDataContext _dataContext;

    public SqlServerDataProvider()
        : this(null, null, null)
    {
    }

    public SqlServerDataProvider(string database, string username, string passeword)
    {
        var connectionString =
              $"Server=.;" +
              $"Database={database ?? $"testdb -{Guid.NewGuid()}"};" +
              $"User Id={username ?? "sa"};" +
              $"Password={passeword ?? "sa(!)Password"};" +
              $"TrustServerCertificate=True";
        _dataContext = new SqlServerDataContext(connectionString);
        var created = _dataContext.Database.EnsureCreated();
        if (created)
        {
            new SqlServerDataSeeder().Seed(_dataContext);
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
