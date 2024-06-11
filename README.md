# aqua-accesscontrol

[![Github Workflow][pub-badge]][pub-link]

| branch | package                                             | AppVeyor                | Travis CI                      |
| ---    | ---                                                 | ---                     | ---                            |
| `main` | [![NuGet Badge][1]][2] [![MyGet Pre Release][3]][4] | [![Build status][5]][6] | [![Travis build Status][7]][8] |

## Description

Query filters for linq expressions.

## Samples

### Global Predicate

Predicates can be based on progam logic or on expanded data query.

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

The following predicate filters out records which have not tenant ID 1.

```C#
var query =
    from o in repo.Orders
    select new { o.Id };
var result = query
    .Apply(Predicate.Create<Order>(o => o.TenantId == 1))
    .ToList();
```

### Property Predicate

The following predicate retrieves product prices only for records which have tenant ID 1, other records have the `Price` property not set.

```C#
var query =
    from p in repo.Products
    select new { p.Id, p.Price };
var result = query
    .Apply(Predicate.Create<Product, decimal>(p => p.Price, p => p.TenantId == 1))
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

[pub-badge]: https://github.com/6bee/aqua-accesscontrol/actions/workflows/publish.yml/badge.svg
[pub-link]: https://github.com/6bee/aqua-accesscontrol/actions/workflows/publish.yml