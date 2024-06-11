// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl;

using Aqua.AccessControl.Predicates;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

[EditorBrowsable(EditorBrowsableState.Never)]
public static class QueryableExtensions
{
    public static IQueryable<T> Apply<T>(this IQueryable<T> queryable, IEnumerable<IPredicate> predicates)
    {
        Assert.ArgumentNotNull(queryable, nameof(queryable));
        Assert.ArgumentNotNull(predicates, nameof(predicates));

        var expression = queryable.Expression.Apply(predicates);
        return ReferenceEquals(expression, queryable.Expression)
            ? queryable
            : new Q<T>(expression, queryable.Provider);
    }

    public static IQueryable<T> Apply<T>(this IQueryable<T> queryable, params IPredicate[] predicates)
        => queryable.Apply((IEnumerable<IPredicate>)predicates);

    private sealed class Q<T> : IQueryable<T>
    {
        public Q(Expression expression, IQueryProvider provider)
        {
            Expression = expression;
            Provider = provider;
        }

        public Expression Expression { get; }

        public Type ElementType => typeof(T);

        public IQueryProvider Provider { get; }

        public IEnumerator<T> GetEnumerator()
            => Provider.CreateQuery<T>(Expression).GetEnumerator();

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();
    }
}
