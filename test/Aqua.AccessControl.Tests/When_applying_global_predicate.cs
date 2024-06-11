// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests;

using Aqua.AccessControl.Predicates;
using Aqua.AccessControl.Tests.DataModel;
using Shouldly;
using System.Linq;
using Xunit;

public class When_applying_global_predicate : PredicateTest
{
    private const string GrantedUser = "test.user1";
    private const string NotGrantedUser = "test.user2";

    protected override IDataProvider DataProvider { get; } = new InMemoryDataProvider();

    [Theory]
    [InlineData(true, 4)]
    [InlineData(false, 0)]
    public void Should_apply_global_predicate(bool predicate, int expectedNumberOfRecords)
    {
        var query = DataProvider.Products;

        var result = query
            .Apply(Predicate.Create(() => predicate == true))
            .Count();

        result.ShouldBe(expectedNumberOfRecords);
    }

    [Theory]
    [InlineData(GrantedUser, 4)]
    [InlineData(NotGrantedUser, 0)]
    public void Should_apply_global_predicate_referencing_entity_not_part_of_basic_query(string username, int expectedNumberOfRecords)
    {
        var query = DataProvider.Products;

        var result = query
            .Apply(Predicate.Create(() =>
                DataProvider.Claims.Any(c =>
                    c.Type == ClaimTypes.Tenant &&
                    c.Value == "1" &&
                    c.Subject == username)))
            .Count();

        result.ShouldBe(expectedNumberOfRecords);
    }
}
