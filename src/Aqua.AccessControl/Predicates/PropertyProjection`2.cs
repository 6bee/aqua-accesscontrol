// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq.Expressions;
using System.Reflection;

internal sealed class PropertyProjection<T, TProperty> : IPropertyProjection
{
    public PropertyProjection(Expression<Func<T, TProperty>> propertySelector, Expression<Func<T, TProperty>> projection)
    {
        propertySelector.AssertNotNull();
        projection.AssertNotNull();

        if (propertySelector.Body is not MemberExpression memberExpression)
        {
            throw new ArgumentException($"Argument {nameof(propertySelector)} expected to be member expression (x => x.Y)");
        }

        Property = Assert.PropertyInfoArgument(memberExpression.Member, nameof(propertySelector));
        Projection = projection;
    }

    public Type Type => typeof(T);

    public MemberInfo Property { get; }

    public Type PropertyType => typeof(TProperty);

    public LambdaExpression Projection { get; }

    public Expression ApplyTo(Expression expression)
        => PropertyProjectionHelper.Apply(
            [this],
            expression.CheckNotNull(),
            typeof(T));
}
