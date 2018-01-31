// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyProjection<T, P> : IPropertyProjection
    {
        public PropertyProjection(Expression<Func<T, P>> propertySelector, Expression<Func<T, P>> projection)
        {
            Assert.ArgumentNotNull(propertySelector, nameof(propertySelector));

            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"Argument {nameof(propertySelector)} expected to be member expression ({typeof(T).Name} => {typeof(T).Name}.{typeof(P).Name})");
            }

            Property = Assert.PropertyInfoArgument(memberExpression.Member, nameof(propertySelector));
            Projection = Assert.ArgumentNotNull(projection, nameof(projection));
        }

        public Type Type => typeof(T);

        public MemberInfo Property { get; }

        public Type PropertyType => typeof(P);

        public LambdaExpression Projection { get; }

        public Expression ApplyTo(Expression expression)
            => PropertyProjectionHelper.Apply(new[] { this }, expression, typeof(T));
    }
}
