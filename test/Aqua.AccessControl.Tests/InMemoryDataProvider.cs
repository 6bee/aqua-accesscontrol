// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using Aqua.AccessControl.Tests.DataModel;
using System.Collections.Generic;
using System.Linq;

public class InMemoryDataProvider : Disposable, IDataProvider
{
    private readonly IEnumerable<Tenant> _tenants;
    private readonly IEnumerable<Claim> _claims;
    private readonly IEnumerable<ProductCategory> _productCategories;
    private readonly IEnumerable<Product> _products;
    private readonly IEnumerable<Order> _orders;

    public InMemoryDataProvider()
    {
        _tenants = new[]
        {
            new Tenant { Id = 1, Name = "Tenant 1" },
            new Tenant { Id = 2, Name = "Tenant 2" },
        };

        _claims = new[]
        {
            new Claim { Id = 1, TenantId = 1, Type = ClaimTypes.Tenant, Value = "1", Subject = "test.user1" },
            new Claim { Id = 2, TenantId = 2, Type = ClaimTypes.Tenant, Value = "2", Subject = "test.user2" },
            new Claim { Id = 3, TenantId = 1, Type = ClaimTypes.EntityAccess.Read, Value = nameof(ProductCategory), Subject = "test.user1" },
            new Claim { Id = 4, TenantId = 1, Type = ClaimTypes.EntityAccess.Read, Value = nameof(Product), Subject = "test.user1" },
        };

        _productCategories = new[]
        {
            new ProductCategory { Id = 11, Name = "Product Category 11", TenantId = 1 },
            new ProductCategory { Id = 12, Name = "Product Category 12", TenantId = 1 },
            new ProductCategory { Id = 21, Name = "Product Category 21", TenantId = 2 },
            new ProductCategory { Id = 22, Name = "Product Category 22", TenantId = 2 },
        };

        ProductCategory Cat(long id) => _productCategories.Single(x => x.Id == id);

        _products = new[]
        {
            new Product { Id = 110, Name = "Product 110", ProductCategory = Cat(11), TenantId = 1, Price = 10m },
            new Product { Id = 120, Name = "Product 120", ProductCategory = Cat(12), TenantId = 1, Price = 100m },
            new Product { Id = 210, Name = "Product 210", ProductCategory = Cat(21), TenantId = 2, Price = 2000m },
            new Product { Id = 211, Name = "Product 211", ProductCategory = Cat(21), TenantId = 2, Price = 20000m },
        };

        _orders = new[]
        {
            new Order
            {
                Id = 1000, TenantId = 1, Items = new[]
                {
                    new OrderItem { Id = 1110, Quantity = 1, ProductId = 110, Price = 10m },
                    new OrderItem { Id = 1120, Quantity = 1, ProductId = 120, Price = 100m },
                },
            },
            new Order
            {
                Id = 2000, TenantId = 2, Items = new[]
                {
                    new OrderItem { Id = 2210, Quantity = 1, ProductId = 210, Price = 200m },
                    new OrderItem { Id = 2211, Quantity = 1, ProductId = 211, Price = 2000m },
                },
            },
        };

        foreach (var order in _orders)
        {
            foreach (var item in order.Items)
            {
                item.OrderId = order.Id;
            }
        }
    }

    public IQueryable<Tenant> Tenants => _tenants.AsQueryable();

    public IQueryable<Claim> Claims => _claims.AsQueryable();

    public IQueryable<ProductCategory> ProductCategories => _productCategories.AsQueryable();

    public IQueryable<Product> Products => _products.AsQueryable();

    public IQueryable<Order> Orders => _orders.AsQueryable();
}
