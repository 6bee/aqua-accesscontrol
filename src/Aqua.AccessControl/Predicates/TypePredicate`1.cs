// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq.Expressions;

internal sealed class TypePredicate<T> : ITypePredicate
{
    private static readonly bool _isSingleElement = TypeHelper.GetElementType(typeof(T)) is null;

    public TypePredicate(Expression<Func<T, bool>> predicate)
        => Predicate = predicate.CheckNotNull();

    public LambdaExpression Predicate { get; }

    public Type Type => typeof(T);

    public Expression ApplyTo(Expression expression)
        => TypePredicateHelper.Apply(
            this,
            expression.CheckNotNull(),
            Type,
            _isSingleElement);
}