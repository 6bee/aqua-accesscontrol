// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

internal sealed class PropertyPredicate : IPropertyPredicate
{
    public PropertyPredicate(MemberInfo property, Type type, Type propertyType, LambdaExpression predicate)
    {
        Type = type.CheckNotNull();
        Property = Assert.PropertyInfoArgument(property);
        PropertyType = propertyType.CheckNotNull();
        Predicate = predicate.CheckNotNull();
    }

    public Type Type { get; }

    public MemberInfo Property { get; }

    public Type PropertyType { get; }

    public LambdaExpression Predicate { get; }

    public Expression ApplyTo(Expression expression)
    {
        expression.CheckNotNull();
        var propertyProjection = PropertyProjectionHelper.ToProjections(new[] { this }).Single();
        return propertyProjection.ApplyTo(expression);
    }
}
