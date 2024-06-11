// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using Aqua.AccessControl.Predicates;
using Aqua.AccessControl.Tests.DataModel;
using Shouldly;
using System.Linq;
using Xunit;

public class When_applying_property_predicate : PredicateTest
{
    protected override IDataProvider DataProvider { get; } = new InMemoryDataProvider();

    [Fact]
    public void Should_filter_out_value_if_not_permitted_property_access()
    {
        var repo = DataProvider;

        var query =
            from p in repo.Products
            select new { p.Id, p.Price };

        var result = query
            .Apply(Predicate.Create<Product, decimal>(p => p.Price, p => p.TenantId == 1))
            .ToList();

        result.Count.ShouldBe(4);
        result.Single(x => x.Id == 110).Price.ShouldBe(10m);
        result.Single(x => x.Id == 120).Price.ShouldBe(100m);
        result.Single(x => x.Id == 210).Price.ShouldBe(default(decimal));
        result.Single(x => x.Id == 211).Price.ShouldBe(default(decimal));
    }

    [Fact]
    public void Should_filter_out_value_if_not_permitted_property_access_by_multiple_property_predicates()
    {
        var repo = DataProvider;

        var query = repo.Products;

        var result = query
            .Apply(
                Predicate.Create<Product, decimal>(p => p.Price, p => p.TenantId == 2),
                Predicate.Create<Product, decimal>(p => p.Price, p => p.Price < 10000))
            .ToList();

        result.Count.ShouldBe(4);
        result.Single(x => x.Id == 110).Price.ShouldBe(default(decimal));
        result.Single(x => x.Id == 120).Price.ShouldBe(default(decimal));
        result.Single(x => x.Id == 210).Price.ShouldBe(2000m);
        result.Single(x => x.Id == 211).Price.ShouldBe(default(decimal));
    }

    [Fact]
    public void Should_apply_property_predicate_on_inherited_property_declared_on_base_type()
    {
        var repo = DataProvider;

        var query = repo.Products;

        var result = query
            .Apply(Predicate.Create<AggregateRoot, long>(x => x.Id, x => x.Id == 110))
            .ToArray();

        result.Length.ShouldBe(4);
        result[0].Id.ShouldBe(110L);
        result[1].Id.ShouldBe(default(long));
        result[2].Id.ShouldBe(default(long));
        result[3].Id.ShouldBe(default(long));
    }

    [Fact]
    public void Should_apply_property_predicate_on_inherited_property()
    {
        var repo = DataProvider;

        var query = repo.Products;

        var result = query
            .Apply(Predicate.Create<Product, long>(x => x.Id, x => x.Id == 110))
            .ToArray();

        result.Length.ShouldBe(4);
        result[0].Id.ShouldBe(110L);
        result[1].Id.ShouldBe(default(long));
        result[2].Id.ShouldBe(default(long));
        result[3].Id.ShouldBe(default(long));
    }

    [Fact]
    public void Should_not_apply_property_predicate_declared_on_other_subtype_for_inherited_property()
    {
        var repo = DataProvider;

        var query = repo.Products;

        var result = query
            .Apply(Predicate.Create<ProductCategory, long>(x => x.Id, x => x.Id == 1))
            .ToArray();

        result.Length.ShouldBe(4);
        result[0].Id.ShouldBe(110L);
        result[1].Id.ShouldBe(120L);
        result[2].Id.ShouldBe(210L);
        result[3].Id.ShouldBe(211L);
    }
}
