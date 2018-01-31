// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyPredicate<T, P> : IPropertyPredicate
    {
        public PropertyPredicate(Expression<Func<T, P>> propertySelector, Expression<Func<T, bool>> predicate)
        {
            Assert.ArgumentNotNull(propertySelector, nameof(propertySelector));

            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"Argument {nameof(propertySelector)} expected to be member expression ({typeof(T).Name} => {typeof(T).Name}.{typeof(P).Name})");
            }

            Property = Assert.PropertyInfoArgument(memberExpression.Member, nameof(propertySelector));
            Predicate = Assert.ArgumentNotNull(predicate, nameof(predicate));
        }

        public Type Type => typeof(T);

        public MemberInfo Property { get; }

        public Type PropertyType => typeof(P);

        public LambdaExpression Predicate { get; }

        public Expression ApplyTo(Expression expression)
        {
            var propertyProjection = PropertyProjectionHelper.ToProjections(new[] { this }).Single();
            return propertyProjection.ApplyTo(expression);
        }
    }
}
