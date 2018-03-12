// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyPredicate : IPropertyPredicate
    {
        public PropertyPredicate(MemberInfo property, Type type, Type propertyType, LambdaExpression predicate)
        {
            Assert.ArgumentNotNull(property, nameof(property));

            Type = Assert.ArgumentNotNull(type, nameof(type));
            Property = Assert.PropertyInfoArgument(property, nameof(property));
            PropertyType = Assert.ArgumentNotNull(propertyType, nameof(propertyType));
            Predicate = Assert.ArgumentNotNull(predicate, nameof(predicate));
        }

        public Type Type { get; }

        public MemberInfo Property { get; }

        public Type PropertyType { get; }

        public LambdaExpression Predicate { get; }

        public Expression ApplyTo(Expression expression)
        {
            var propertyProjection = PropertyProjectionHelper.ToProjections(new[] { this }).Single();
            return propertyProjection.ApplyTo(expression);
        }
    }
}
