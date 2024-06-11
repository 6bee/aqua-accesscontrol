// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using Aqua.AccessControl.Predicates;
using Aqua.AccessControl.Tests.DataModel;
using Shouldly;
using System.Linq;
using Xunit;

public abstract class When_applying_type_predicate : PredicateTest
{
    protected override IDataProvider DataProvider { get; } = new InMemoryDataProvider();

    [Fact]
    public void Should_appy_predicate_to_referenced_entity()
    {
        var repo = DataProvider;

        var query = (
            from p in repo.Products
            select p.ProductCategory)
            .Distinct();

        var result = query
            .Apply(Predicate.Create<ProductCategory>(x => x.Name == "Product Category 21"))
            .ToList();

        result.ShouldHaveSingleItem()
            .Id.ShouldBe(21);
    }

    [Fact]
    public void Should_appy_predicate_to_referenced_entity_selecting_id_property2()
    {
        var repo = DataProvider;

        var query = (
            from p in repo.Products
            select p.ProductCategory.Id)
            .Distinct();

        var result = query
            .Apply(Predicate.Create<ProductCategory>(x => x.Name == "Product Category 21"))
            .ToList();

        result.ShouldHaveSingleItem()
            .ShouldBe(21);
    }

    [Fact]
    public void Should_appy_predicate_to_referenced_entity_selecting_id_property3()
    {
        var repo = DataProvider;

        var query = (
            from p in repo.Products
            select new { p.ProductCategory.Id })
            .Distinct();

        var result = query
            .Apply(Predicate.Create<ProductCategory>(x => x.Name == "Product Category 21"))
            .ToList();

        result.ShouldHaveSingleItem()
            .Id.ShouldBe(21);
    }

    [Fact]
    public void Should_apply_predicate_on_inherited_property()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            select new { o.Id };

        var result = query
            .Apply(Predicate.Create<AggregateRoot>(p => p.TenantId == 1))
            .ToList();

        result.ShouldHaveSingleItem()
            .Id.ShouldBe(1000);
    }

    [Fact]
    public void Should_apply_predicate_declared_on_subtype_on_inherited_property()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            select new { o.Id };

        var result = query
            .Apply(Predicate.Create<Order>(o => o.TenantId == 1))
            .ToList();

        result.ShouldHaveSingleItem()
            .Id.ShouldBe(1000);
    }

    [Fact]
    public void Should_not_apply_predicate_declared_on_other_subtype_on_inherited_property()
    {
        var repo = DataProvider;

        var query =
            from o in repo.Orders
            select new { o.Id };

        var result = query
            .Apply(Predicate.Create<ProductCategory>(p => p.TenantId == 1))
            .ToList();

        result.Count.ShouldBe(2);
    }
}