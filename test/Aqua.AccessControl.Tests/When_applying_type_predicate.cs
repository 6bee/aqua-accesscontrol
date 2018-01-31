// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Tests
{
    using Aqua.AccessControl.Predicates;
    using DataModel;
    using Shouldly;
    using System.Linq;
    using Xunit;

    public class When_applying_type_predicate : PredicateTest
    {
        protected override IDataProvider DataProvider { get; } = new InMemoryDataProvider();

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
        public void Should_appy_predicate_to_child_collection_items()
        {
            var repo = DataProvider;
  
            // TODO: for non in-memory tests: select order instead of orderitem and assert items do match predicate
            var query =
                from o in repo.Orders
                from i in o.Items // hey I'm an IEnumerable<>
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
        public void Should_appy_predicate_to_referenced_entity()
        {
            var repo = DataProvider;

            var query = (
                from p in repo.Products
                select p.ProductCategory // hey I'm a reference to a single element
                ).Distinct();

            var result = query
                .Apply(Predicate.Create<ProductCategory>(x => x.Name == "Product Category 21"))
                .ToList();

            result.ShouldHaveSingleItem()
                .Id.ShouldBe(21);
        }

        [Fact]
        public void Should_apply_predicate_joining_additional_entity_not_part_of_basic_query()
        {
            var repo = DataProvider;

            var query =
                from o in repo.Orders
                from i in o.Items
                join p in repo.Products on i.ProductId equals p.Id
                select i.Id;

            var result = query
                .Apply(Predicate.Create<Product>(p =>
                    repo.Claims.Any(c =>
                        c.TenantId == p.TenantId &&
                        c.Type == ClaimType.EntityAccess.Read &&
                        c.Value == nameof(Product) &&
                        c.Subject == "test.user1")))
                .ToList();

            result.Count.ShouldBe(2);
            result.ShouldContain(1110);
            result.ShouldContain(1120);
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
}