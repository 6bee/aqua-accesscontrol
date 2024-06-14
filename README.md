# aqua-accesscontrol

[![GitHub license][lic-badge]][lic-link]
[![Github Workflow][pub-badge]][pub-link]

| branch | AppVeyor                | Travis CI                      |
| ---    | ---                     | ---                            |
| `main` | [![Build status][5]][6] | [![Travis build Status][7]][8] |

| package              | nuget                  | myget                        |
| ---                  | ---                    | ---                          |
| `aqua-accesscontrol` | [![NuGet Badge][1]][2] | [![MyGet Pre Release][3]][4] |

## Description

This C# library provides extension methods for [System.Linq.IQueryable<>](https://learn.microsoft.com/en-us/dotnet/api/system.linq.iqueryable) and [System.Linq.Expressions.Expression](https://learn.microsoft.com/en-us/dotnet/api/system.linq.expressions.expression) types, to apply query filters at various levels.

## Features

### Global Predicate

Global predicates apply to a query as a whole and must be satisfied for any results be returned.

Predicates can be based on progam logic and/or on expanded data query:

```C#
// base query
var query =
    from p in repo.Products
    select p;
// code based predicate
var result1 = query
    .Apply(Predicate.Create(() => true))
    .ToList();
// predicate based on data qeury
var result2 = query
    .Apply(Predicate.Create(() =>
        repo.Claims.Any(c =>
            c.Type == ClaimTypes.Tenant &&
            c.Value == "1" &&
            c.Subject == username)))
    .ToList();
```

### Type Predicate

Type predicates apply to specific record types within a query by filtering out corresponding records that do not satisfy the condition.

The following predicate filters out records which have not `TenantId` equal to 1:

```C#
var query =
    from o in repo.Orders
    select new { o.Id };
var result = query
    .Apply(Predicate.Create<Order>(o => o.TenantId == 1))
    .ToList();
```

### Property Predicate

Property predicates do not filter out records but allow property values to be returned only when specified conditions are satisfied.

The following predicate retrieves product prices only for records which have `TenantId` equal to 1, other records have the `Price` property set to its default vaule:

```C#
var query =
    from p in repo.Products
    select new { p.Id, p.Price };
var result = query
    .Apply(Predicate.Create<Product, decimal>(
        p => p.Price,
        p => p.TenantId == 1))
    .ToList();
```

### Property Projection Predicate

Property projection predicates allow to project values of a certain property based on custom logic.

In the following example, a 10% discount is applied if `TenantId` is equal to 1:

```C#
var query =
    from p in repo.Products
    select new { p.Id, p.Price };

var result = query
    .Apply(Predicate.CreatePropertyProjection<Product, decimal>(
        p => p.Price,
        p => p.TenantId == 1 ? p.Price * 0.9m : p.Price))
    .ToList();
```

[1]: https://buildstats.info/nuget/aqua-accesscontrol?includePreReleases=true
[2]: http://www.nuget.org/packages/aqua-accesscontrol
[3]: http://img.shields.io/myget/aqua/vpre/aqua-accesscontrol.svg?style=flat-square&label=myget
[4]: https://www.myget.org/feed/aqua/package/nuget/aqua-accesscontrol
[5]: https://ci.appveyor.com/api/projects/status/se738mykuhel4b3q/branch/main?svg=true
[6]: https://ci.appveyor.com/project/6bee/aqua-accesscontrol/branch/main
[7]: https://travis-ci.org/6bee/aqua-accesscontrol.svg?branch=main
[8]: https://travis-ci.org/6bee/aqua-accesscontrol?branch=main

[lic-badge]: https://img.shields.io/github/license/6bee/aqua-accesscontrol.svg
[lic-link]: https://github.com/6bee/aqua-accesscontrol/blob/main/license.txt

[pub-badge]: https://github.com/6bee/aqua-accesscontrol/actions/workflows/publish.yml/badge.svg
[pub-link]: https://github.com/6bee/aqua-accesscontrol/actions/workflows/publish.yml
