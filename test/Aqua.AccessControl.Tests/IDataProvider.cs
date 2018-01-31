// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests
{
    using Aqua.AccessControl.Tests.DataModel;
    using System;
    using System.Linq;

    public interface IDataProvider : IDisposable
    {
        IQueryable<Tenant> Tenants { get; }
        IQueryable<Claim> Claims { get; }
        IQueryable<ProductCategory> ProductCategories { get; }
        IQueryable<Product> Products { get; }
        IQueryable<Order> Orders { get; }
    }
}
