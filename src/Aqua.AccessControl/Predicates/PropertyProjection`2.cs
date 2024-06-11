// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq.Expressions;
using System.Reflection;

internal sealed class PropertyProjection<T, TProperty> : IPropertyProjection
{
    public PropertyProjection(Expression<Func<T, TProperty>> propertySelector, Expression<Func<T, TProperty>> projection)
    {
        Assert.ArgumentNotNull(propertySelector, nameof(propertySelector));

        var memberExpression = propertySelector.Body as MemberExpression;
        if (memberExpression is null)
        {
            throw new ArgumentException($"Argument {nameof(propertySelector)} expected to be member expression (x => x.Y)");
        }

        Property = Assert.PropertyInfoArgument(memberExpression.Member, nameof(propertySelector));
        Projection = Assert.ArgumentNotNull(projection, nameof(projection));
    }

    public Type Type => typeof(T);

    public MemberInfo Property { get; }

    public Type PropertyType => typeof(TProperty);

    public LambdaExpression Projection { get; }

    public Expression ApplyTo(Expression expression)
        => PropertyProjectionHelper.Apply(
            new[] { this },
            Assert.ArgumentNotNull(expression, nameof(expression)),
            typeof(T));
}
