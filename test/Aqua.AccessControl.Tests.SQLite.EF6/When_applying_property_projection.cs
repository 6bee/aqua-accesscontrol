// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SQLite.EF6;

using Aqua.AccessControl.Predicates;
using Aqua.AccessControl.Tests.DataModel;
using Shouldly;
using System;
using System.Linq;
using Xunit;

public class When_applying_property_projection : PredicateTest
{
    protected override IDataProvider DataProvider { get; } = new SQLiteDataProvider();

    [Fact]
    public void Should_throw_since_ef_does_not_allow_projection_into_entity_type()
    {
        var repo = DataProvider;
        var query = repo.Products.Apply(Predicate.CreatePropertyProjection<Product, long>(x => x.Id, x => x.Id));

        var ex = Assert.Throws<NotSupportedException>(() => query.ToList());
        ex.Message.ShouldBe($"The entity or complex type '{typeof(SQLiteDataProvider).Namespace}.{nameof(Product)}' cannot be constructed in a LINQ to Entities query.");
    }
}
