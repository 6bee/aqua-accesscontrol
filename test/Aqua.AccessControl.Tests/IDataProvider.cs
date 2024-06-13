// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

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

    IQueryable<Parent> Parents { get; }

    IQueryable<Child> Children { get; }
}