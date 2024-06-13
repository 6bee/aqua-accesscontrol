// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.EFCore.SqlServer;

using Aqua.AccessControl.Predicates;
using Aqua.AccessControl.Tests.DataModel;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using System.Linq;
using Xunit;

public class When_applying_type_predicate : Tests.When_applying_type_predicate
{
    protected override IDataProvider DataProvider { get; } = new SqlServerDataProvider();

    [Fact]
    public void Should_apply_predicate_to_joined_entity_not_part_of_select()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            from i in o.Items
            join p in repo.Products on i.ProductId equals p.Id
            select i;

        var result = query
            .Apply(Predicate.Create<Product>(p => p.ProductCategory.Id == 11))
            .ToList();

        result.ShouldHaveSingleItem()
            .ProductId.ShouldBe(110);
    }

    [Fact]
    public void Should_apply_predicate_joining_entity_not_part_of_select()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            from i in o.Items
            select i;

        var result = query
            .Apply(Predicate.Create<OrderItem>(
                i => repo.Products.Any(
                    p => p.Id == i.ProductId && p.ProductCategory.Id == 11)))
            .ToList();

        result.ShouldHaveSingleItem()
            .ProductId.ShouldBe(110);
    }

    [Fact]
    public void Should_apply_predicate_joining_additional_entities_not_part_of_basic_query()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            from i in o.Items
            select i.Id;

        var result = query
            .Apply(Predicate.Create<OrderItem>(i =>
                repo.Products.Any(p => p.Id == i.ProductId &&
                    repo.Claims.Any(c =>
                        c.TenantId == p.TenantId &&
                        c.Type == ClaimTypes.EntityAccess.Read &&
                        c.Value == nameof(Product) &&
                        c.Subject == "test.user1"))))
            .ToList();

        result.Count.ShouldBe(2);
        result.ShouldContain(1110);
        result.ShouldContain(1120);
    }

    [Fact]
    public void Should_appy_predicate_targeting_child_collection_items_not_part_of_query()
    {
        var repo = DataProvider;

        var query = repo.Orders.Select(x => x.Id);

        var result = query
            .Apply(Predicate.Create<Order>(x => x.Items.All(i => i.Price < 1000)))
            .ToList();

        result.Count.ShouldBe(1);
        result.ShouldContain(1000);
    }

    [Fact]
    public void Should_appy_predicate_to_child_collection_items()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            from i in o.Items
            select i.Id;

        var result = query
            .Apply(Predicate.Create<OrderItem>(x => x.Price < 1000))
            .ToList();

        result.Count.ShouldBe(3);
        result.ShouldContain(1110);
        result.ShouldContain(1120);
        result.ShouldContain(2210);
    }

    [Fact]
    public void Should_appy_predicate_to_child_collection_items_queried_via_select_many()
    {
        var repo = DataProvider;

        var query = repo.Orders.SelectMany(x => x.Items).Select(x => x.Id);

        var result = query
            .Apply(Predicate.Create<OrderItem>(x => x.Price < 1000))
            .ToList();

        result.Count.ShouldBe(3);
        result.ShouldContain(1110);
        result.ShouldContain(1120);
        result.ShouldContain(2210);
    }

    [Fact]
    public void Should_appy_predicate_targeting_child_collection_items_with_eager_loading()
    {
        var repo = DataProvider;

        var query = repo.Orders.Include(x => x.Items);

        var result = query
            .Apply(Predicate.Create<Order>(x => x.Items.All(i => i.Price < 1000)))
            .ToList();

        result.ShouldHaveSingleItem()
            .Id.ShouldBe(1000);
    }

    [Fact]
    public void Should_appy_predicate_targeting_child_collection_items_without_eager_loading()
    {
        var repo = DataProvider;

        var query = repo.Orders;

        var result = query
            .Apply(Predicate.Create<Order>(x => x.Items.All(i => i.Price < 1000)))
            .ToList();

        result.ShouldHaveSingleItem()
            .Id.ShouldBe(1000);
    }

    [Fact]
    public void Should_appy_predicate_to_child_collection_added_via_include()
    {
        var repo = DataProvider;

        IQueryable<Order> query = repo.Orders
            .Include(x => x.Items);

        query = query.Apply(Predicate.Create<OrderItem>(x => x.Price > 10 && x.Price < 1000));

        var result = query.ToList();

        var items = result.SelectMany(x => x.Items).Select(x => x.Id).ToList();
        items.Count.ShouldBe(2);
        items.ShouldContain(1120);
        items.ShouldContain(2210);
    }

    [Fact]
    public void Should_appy_predicate_to_child_collection_added_via_include_with_splitquery()
    {
        var repo = DataProvider;

        IQueryable<Order> query = repo.Orders
            .Include(x => x.Items)
            .AsSplitQuery();

        query = query.Apply(Predicate.Create<OrderItem>(x => x.Price > 10 && x.Price < 1000));

        var result = query.ToList();

        var items = result.SelectMany(x => x.Items).Select(x => x.Id).ToList();
        items.Count.ShouldBe(2);
        items.ShouldContain(1120);
        items.ShouldContain(2210);
    }

    [Fact]
    public void Should_appy_predicate_to_child_collection_added_via_then_include()
    {
        var repo = DataProvider;

        IQueryable<Child> query = repo.Children
            .Include(x => x.Parent)
            .ThenInclude(x => x.Children);

        query = query.Apply(Predicate.Create<Child>(x => x.Id == 21));

        var result = query.ToList();

        result.ShouldHaveSingleItem()
            .Parent.ShouldNotBeNull()
            .Children.ShouldHaveSingleItem()
            .Id.ShouldBe(21);
    }

    [Fact(Skip = "InvalidOperationException : The expression 'IIF((x.Parent.Id == 2), x.Parent, null)' is invalid inside an 'Include' operation")]
    public void Should_not_throw_upon_applying_predicate_to_reference_property_added_via_include()
    {
        // 'filtered include' for reference property (non-collection navigation property)
        // is currently not supported by EF core: https://github.com/dotnet/efcore/issues/24422

        var repo = DataProvider;

        IQueryable<Child> query = repo.Children.Include(child => child.Parent);

        query = query.Apply(Predicate.Create<Parent>(parent => parent.Id == 2));

        var result = query.ToList();
    }
}