// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

internal sealed class Queryable<T> : IQueryable<T>
{
    public Queryable(Expression expression, IQueryProvider provider)
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