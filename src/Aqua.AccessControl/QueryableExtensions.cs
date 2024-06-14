// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl;

using Aqua.AccessControl.Predicates;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class QueryableExtensions
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> queryable, IEnumerable<IPredicate> predicates)
    {
        queryable.AssertNotNull();
        predicates.AssertNotNull();

        var expression = queryable.Expression.Apply(predicates);
        return ReferenceEquals(expression, queryable.Expression)
            ? queryable
            : new Queryable<T>(expression, queryable.Provider);
    }

    public static IQueryable<T> Apply<T>(this IQueryable<T> queryable, params IPredicate[] predicates)
        => queryable.Apply((IEnumerable<IPredicate>)predicates);
}