// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System;
    using System.Linq.Expressions;

    internal class TypePredicate<T> : ITypePredicate
    {
        public TypePredicate(Expression<Func<T, bool>> predicate)
            => Predicate = Assert.ArgumentNotNull(predicate, nameof(predicate));

        public LambdaExpression Predicate { get; }

        public Type Type => typeof(T);

        public Expression ApplyTo(Expression expression)
            => TypePredicateHelper.Apply(this, expression, Type, isSingleElement: TypeHelper.GetElementType(Type) == null);
    }
}
