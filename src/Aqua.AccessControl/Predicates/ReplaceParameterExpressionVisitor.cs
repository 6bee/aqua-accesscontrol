// Copyright (c) Christof Senn. All rights reserved. 

namespace Aqua.AccessControl.Predicates
{
    using System.Collections.Generic;
    using System.Linq.Expressions;

    internal class ReplaceParameterExpressionVisitor : ExpressionVisitor
    {
        private readonly IDictionary<ParameterExpression, Expression> _parameterMap;

        public ReplaceParameterExpressionVisitor(IDictionary<ParameterExpression, Expression> parameterMap)
            => _parameterMap = Assert.ArgumentNotNull(parameterMap, nameof(parameterMap));

        protected override Expression VisitParameter(ParameterExpression node)
            => _parameterMap.TryGetValue(node, out Expression expression) ? expression : node;
    }
}
