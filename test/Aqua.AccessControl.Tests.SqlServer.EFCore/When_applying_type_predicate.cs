// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Tests.SqlServer.EFCore
{
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
        public void Should_appy_predicate_targeting_child_collection_items_added_via_include()
        {
            var repo = DataProvider;

            var query = repo.Orders.Include(x => x.Items);

            var result = query
                .Apply(Predicate.Create<Order>(x => x.Items.All(i => i.Price < 1000)))
                .ToList()
                .SelectMany(x => x.Items)
                .Select(x => x.Id)
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

        [Fact(Skip = "Expression of type 'System.Collections.Generic.IEnumerable`1[Aqua.AccessControl.Tests.DataModel.OrderItem]' cannot be used for return type 'System.Collections.Generic.ICollection`1[Aqua.AccessControl.Tests.DataModel.OrderItem]'")]
        public void Should_not_throw_upon_appying_predicate_to_child_collection_added_via_include()
        {
            // var repo = DataProvider;

            // var query = repo.Orders.Include(x => x.Items);

            // var ex = Should.Throw<ArgumentException>(() => query.Apply(Predicate.Create<OrderItem>(x => x.Price < 1000)));
            // ex.Message.ShouldBe("Expression of type 'System.Collections.Generic.IEnumerable`1[Aqua.AccessControl.Tests.DataModel.OrderItem]' cannot be used for return type 'System.Collections.Generic.ICollection`1[Aqua.AccessControl.Tests.DataModel.OrderItem]'");

            // var result = query
            //    .Apply(Predicate.Create<OrderItem>(x => x.Price < 1000))
            //    .ToList();
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
    }
}
