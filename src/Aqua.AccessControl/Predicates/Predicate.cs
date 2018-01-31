// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq.Expressions;

    public static class Predicate
    {
        public static IPredicate Create(Expression<Func<bool>> predicate)
            => new GlobalPredicate(predicate);

        public static ITypePredicate Create<T>(Expression<Func<T, bool>> predicate)
            => new TypePredicate<T>(predicate);

        public static IPropertyPredicate Create<T, P>(Expression<Func<T, P>> propertySelector, Expression<Func<T, bool>> predicate)
            => new PropertyPredicate<T, P>(propertySelector, predicate);

        /// <summary>
        /// Custom logic for projection of property value
        /// </summary>
        /// <remarks><see cref="IPropertyProjection"/> may not be combined with <see cref="IPropertyPredicate"/> within the same query</remarks>
        public static IPropertyProjection CreatePropertyProjection<T, P>(Expression<Func<T, P>> propertySelector, Expression<Func<T, P>> projector) 
            => new PropertyProjection<T, P>(propertySelector, projector);
    }
}
