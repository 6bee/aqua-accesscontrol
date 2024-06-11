// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq.Expressions;
using System.Reflection;

internal sealed class PropertyProjection : IPropertyProjection
{
    internal PropertyProjection(MemberInfo property, Type type, Type propertyType, LambdaExpression projection)
    {
        Type = type.CheckNotNull();
        Property = Assert.PropertyInfoArgument(property);
        PropertyType = propertyType.CheckNotNull();
        Projection = projection.CheckNotNull();
    }

    public Type Type { get; }

    public MemberInfo Property { get; }

    public Type PropertyType { get; }

    public LambdaExpression Projection { get; }

    public Expression ApplyTo(Expression expression)
        => PropertyProjectionHelper.Apply(
            [this],
            expression.CheckNotNull(),
            Property.DeclaringType);
}
