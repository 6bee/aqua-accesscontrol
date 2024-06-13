// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using Aqua.AccessControl.Predicates;
using Aqua.AccessControl.Tests.DataModel;
using Shouldly;
using System;
using System.Linq;
using Xunit;

public class When_applying_property_projection : PredicateTest
{
    protected override IDataProvider DataProvider { get; } = new InMemoryDataProvider();

    [Fact]
    public void Should_throw_if_more_than_one_projection_is_specifdied_for_same_property()
    {
        var projection1 = Predicate.CreatePropertyProjection<Product, long>(x => x.Id, x => x.Id);

        var repo = DataProvider;

        var queryable = repo.Products;
        var ex = Assert.Throws<ArgumentException>(() => queryable.Apply(projection1, projection1));
        ex.GetCleanMessage().ShouldBe($"Multiple predicates and/or projections defined for property: {typeof(Product).FullName}.{nameof(Product.Id)} (Parameter 'predicates')");
    }

    [Fact]
    public void Should_throw_if_projection_and_predicate_is_specifdied_for_same_property()
    {
        var projection = Predicate.CreatePropertyProjection<AggregateRoot, long>(x => x.Id, x => x.Id);
        var predicate = Predicate.Create<AggregateRoot, long>(x => x.Id, x => x.TenantId == 1);

        var repo = DataProvider;
        var queryable = repo.Products;
        var ex = Assert.Throws<ArgumentException>(() => queryable.Apply(projection, predicate));
        ex.GetCleanMessage().ShouldBe($"Multiple predicates and/or projections defined for property: {typeof(AggregateRoot).FullName}.{nameof(AggregateRoot.Id)} (Parameter 'predicates')");
    }

    [Fact]
    public void Should_replace_value_according_property_projection()
    {
        var repo = DataProvider;

        var query =
            from p in repo.Products
            select new { p.Id, p.Price };

        var result = query
            .Apply(Predicate.CreatePropertyProjection<Product, decimal>(p => p.Price, p => p.TenantId == 1 ? default : p.Price * 2))
            .ToList();

        result.Count.ShouldBe(4);
        result.Single(x => x.Id == 110).Price.ShouldBe(default(decimal));
        result.Single(x => x.Id == 120).Price.ShouldBe(default(decimal));
        result.Single(x => x.Id == 210).Price.ShouldBe(4000m);
        result.Single(x => x.Id == 211).Price.ShouldBe(40000m);
    }

    [Fact]
    public void Should_project_property_value_when_multiple_property_projections_applied()
    {
        var repo = DataProvider;

        var query =
            from p in repo.Products
            select new { p.Id, p.Price };

        var result = query
            .Apply(
                Predicate.CreatePropertyProjection<Product, long>(p => p.Id, p => p.TenantId),
                Predicate.CreatePropertyProjection<Product, decimal>(p => p.Price, p => p.TenantId))
            .ToArray();

        result.Length.ShouldBe(4);
        result[0].Id.ShouldBe(1L);
        result[0].Price.ShouldBe(1M);
        result[1].Id.ShouldBe(1L);
        result[1].Price.ShouldBe(1M);
        result[2].Id.ShouldBe(2L);
        result[2].Price.ShouldBe(2M);
        result[3].Id.ShouldBe(2L);
        result[3].Price.ShouldBe(2M);
    }

    [Fact]
    public void Should_project_property_value_when_multiple_property_projections_applied_separately()
    {
        var repo = DataProvider;

        var query =
            from p in repo.Products
            select new { p.Id, p.Price };

        var result = query
            .Apply(Predicate.CreatePropertyProjection<Product, long>(p => p.Id, p => p.TenantId))
            .Apply(Predicate.CreatePropertyProjection<Product, decimal>(p => p.Price, p => p.TenantId))
            .ToArray();

        result.Length.ShouldBe(4);
        result[0].Id.ShouldBe(1L);
        result[0].Price.ShouldBe(1M);
        result[1].Id.ShouldBe(1L);
        result[1].Price.ShouldBe(1M);
        result[2].Id.ShouldBe(2L);
        result[2].Price.ShouldBe(2M);
        result[3].Id.ShouldBe(2L);
        result[3].Price.ShouldBe(2M);
    }
}