// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq;
using System.Linq.Expressions;

internal sealed class GlobalPredicate : IPredicate
{
    public GlobalPredicate(Expression<Func<bool>> predicate)
        => Predicate = predicate.CheckNotNull();

    public Expression<Func<bool>> Predicate { get; }

    public Expression ApplyTo(Expression expression)
    {
        expression.AssertNotNull();
        var elementType = expression.Type.GenericTypeArguments.Single();
        var predicate = Expression.Lambda(Predicate.Body, Expression.Parameter(elementType));
        return Expression.Call(MethodInfos.Queryable.Where(elementType), expression, predicate);
    }
}
