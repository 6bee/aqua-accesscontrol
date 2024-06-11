// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq.Expressions;

public static class Predicate
{
    public static IPredicate Create(Expression<Func<bool>> predicate)
        => new GlobalPredicate(predicate);

    public static ITypePredicate Create<T>(Expression<Func<T, bool>> predicate)
        => new TypePredicate<T>(predicate);

    public static IPropertyPredicate Create<T, TProperty>(Expression<Func<T, TProperty>> propertySelector, Expression<Func<T, bool>> predicate)
        => new PropertyPredicate<T, TProperty>(propertySelector, predicate);

    /// <summary>
    /// Custom logic for projection of property value.
    /// </summary>
    /// <remarks><see cref="IPropertyProjection"/> may not be combined with <see cref="IPropertyPredicate"/> within the same query.</remarks>
    public static IPropertyProjection CreatePropertyProjection<T, TProperty>(Expression<Func<T, TProperty>> propertySelector, Expression<Func<T, TProperty>> projector)
        => new PropertyProjection<T, TProperty>(propertySelector, projector);
}
