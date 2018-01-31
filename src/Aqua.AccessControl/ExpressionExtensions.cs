// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl
{
    using Aqua.AccessControl.Predicates;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq.Expressions;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class ExpressionExtensions
    {
        public static Expression Apply(this Expression expression, IEnumerable<IPredicate> predicates)
            => new PredicateExpressionVisitor(predicates).Visit(expression);

        public static Expression Apply(this Expression expression, params IPredicate[] predicates)
            => expression.Apply((IEnumerable<IPredicate>)predicates);
    }
}
