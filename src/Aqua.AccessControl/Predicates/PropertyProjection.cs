// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyProjection : IPropertyProjection
    {
        internal PropertyProjection(MemberInfo property, Type type, Type propertyType, LambdaExpression projection)
        {
            Assert.ArgumentNotNull(property, nameof(property));

            Type = Assert.ArgumentNotNull(type, nameof(type));
            Property = Assert.PropertyInfoArgument(property, nameof(property));
            PropertyType = Assert.ArgumentNotNull(propertyType, nameof(propertyType));
            Projection = Assert.ArgumentNotNull(projection, nameof(projection));
        }

        public Type Type { get; }

        public MemberInfo Property { get; }

        public Type PropertyType { get; }

        public LambdaExpression Projection { get; }

        public Expression ApplyTo(Expression expression)
            => PropertyProjectionHelper.Apply(new[] { this }, expression, Property.DeclaringType);
    }
}
