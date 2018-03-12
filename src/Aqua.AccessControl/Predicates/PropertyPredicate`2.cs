// Copyright (c) Christof Senn. All rights reserved. See license.txt in the project root for license information.

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Reflection;

    internal class PropertyPredicate<T, TProperty> : IPropertyPredicate
    {
        public PropertyPredicate(Expression<Func<T, TProperty>> propertySelector, Expression<Func<T, bool>> predicate)
        {
            Assert.ArgumentNotNull(propertySelector, nameof(propertySelector));

            var memberExpression = propertySelector.Body as MemberExpression;
            if (memberExpression == null)
            {
                throw new ArgumentException($"Argument {nameof(propertySelector)} expected to be member expression (x => x.Y)");
            }

            Property = Assert.PropertyInfoArgument(memberExpression.Member, nameof(propertySelector));
            Predicate = Assert.ArgumentNotNull(predicate, nameof(predicate));
        }

        public Type Type => typeof(T);

        public MemberInfo Property { get; }

        public Type PropertyType => typeof(TProperty);

        public LambdaExpression Predicate { get; }

        public Expression ApplyTo(Expression expression)
        {
            var propertyProjection = PropertyProjectionHelper.ToProjections(new[] { this }).Single();
            return propertyProjection.ApplyTo(expression);
        }
    }
}
